using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using MKEToolInterview.Models;
using MKEToolInterview.Service.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKEToolInterview.Service
{
    public class ReviewService
    {
        private RestaurantService RestaurantService { get; set; }
        private AmazonDynamoDBClient DynamoDBClient { get; set; }
        private const string TableName = "mketool-restaurants";

        public ReviewService(RestaurantService restaurantService, AmazonDynamoDBClient dynamoDbClient)
        {
            RestaurantService = restaurantService;
            DynamoDBClient = dynamoDbClient;
        }

        public async Task<Guid?> CreateNewReview(RestaurantReview review, string restaurantId)
        {
            // First, validate that the restaurant given by the restaurantId exists; if not, bail out.
            // We will also need the restaurant later to update its average rating.
            var restaurant = await RestaurantService.GetRestaurantById(restaurantId);

            if (restaurant == null)
            {
                // If the restaurant doesn't exist, bail out.
                // Indicate that we couldn't create it with a null Guid (which the controller then handles).
                return null;
            }

            review.RestaurantId = Guid.Parse(restaurantId);

            // Generate a guid to act as the review's ID, and apply it to the review object to be stored in dynamo
            review.Id = Guid.NewGuid();

            // Pin the rating to the range of 0 - 5
            review.Rating = Math.Min(Math.Max(0, review.Rating), 5);

            var attributeValues = RestaurantReviewMapper.MapToDynamoAttributes(review);

            var writeRequest = new WriteRequest
            {
                PutRequest = new PutRequest { Item = attributeValues }
            };

            var requestItems = new Dictionary<string, List<WriteRequest>>();
            requestItems[TableName] = new List<WriteRequest> { writeRequest };

            var request = new BatchWriteItemRequest { RequestItems = requestItems };

            await DynamoDBClient.BatchWriteItemAsync(request);

            await UpdateAverageRatingOnRestaurant(restaurant);

            return review.Id;
        }

        public async Task<IEnumerable<RestaurantReview>> GetAllReviewsForRestaurant(string restaurantId)
        {
            var restaurantTable = Table.LoadTable(DynamoDBClient, TableName);
            var queryFilter = new QueryFilter("RestaurantId", QueryOperator.Equal, restaurantId);
            queryFilter.AddCondition("SortKey", QueryOperator.BeginsWith, "Review#");

            var search = restaurantTable.Query(queryFilter);

            var results = new List<Document>();
            do
            {
                results.AddRange(await search.GetNextSetAsync());
            } while (!search.IsDone);

            return results.Select(x => RestaurantReviewMapper.MapFromDynamoDocument(x));
        }

        public async Task<RestaurantReview> GetReviewById(string restaurantId, string reviewId)
        {
            var restaurantTable = Table.LoadTable(DynamoDBClient, TableName);
            var reviewDoc = await restaurantTable.GetItemAsync(restaurantId, $"Review#{reviewId}");

            RestaurantReview review = null;

            if (reviewDoc != null)
            {
                review = RestaurantReviewMapper.MapFromDynamoDocument(reviewDoc);
            }

            return review;
        }

        public async Task UpdateReview(RestaurantReview review)
        {
            var document = RestaurantReviewMapper.MapToDynamoDocument(review);
            var restaurantTable = Table.LoadTable(DynamoDBClient, TableName);

            await restaurantTable.UpdateItemAsync(document);

            var restaurant = await RestaurantService.GetRestaurantById(review.RestaurantId.ToString());

            await UpdateAverageRatingOnRestaurant(restaurant);
        }

        public async Task DeleteReview(string restaurantId, string reviewId)
        {
            var restaurantTable = Table.LoadTable(DynamoDBClient, TableName);
            await restaurantTable.DeleteItemAsync(restaurantId, $"Review#{reviewId}");

            var restaurant = await RestaurantService.GetRestaurantById(restaurantId);

            await UpdateAverageRatingOnRestaurant(restaurant);
        }

        private async Task UpdateAverageRatingOnRestaurant(RestaurantSummary summary)
        {
            var reviews = (await GetAllReviewsForRestaurant(summary.Id.ToString())).ToList();

            var average = reviews.Sum(x => x.Rating) / reviews.Count;

            summary.AverageRating = average;

            await RestaurantService.UpdateRestaurant(summary);
        }
    }
}
