using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using MKEToolInterview.Models;
using MKEToolInterview.Service.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKEToolInterview.Service
{
    // TODOs around the service:
    // - ideally, this would be set up as an interface for better DI into the controller
    // - the dynamo stuff would get pushed down to a DAL and hidden behind and interface so it could get swapped out later

    public class RestaurantService
    {
        private AmazonDynamoDBClient DynamoDBClient { get; set; }
        private const string TableName = "mketool-restaurants";

        public RestaurantService(AmazonDynamoDBClient dynamoDbClient)
        {
            DynamoDBClient = dynamoDbClient;
        }

        public async Task<Guid> CreateNewRestaurant(RestaurantSummary summary)
        {
            // Generate a guid to act as the summary's ID, and apply it to the summary object to be stored in dynamo
            summary.Id = Guid.NewGuid();

            // If the create is trying to set the average rating, clear it out; that will be computed once ratings exist.
            summary.AverageRating = null;

            var attributeValues = RestaurantSummaryMapper.MapToDynamoAttributes(summary);

            var writeRequest = new WriteRequest
            {
                PutRequest = new PutRequest { Item = attributeValues }
            };

            var requestItems = new Dictionary<string, List<WriteRequest>>();
            requestItems[TableName] = new List<WriteRequest> { writeRequest };

            var request = new BatchWriteItemRequest { RequestItems = requestItems };

            await DynamoDBClient.BatchWriteItemAsync(request);

            return summary.Id;
        }

        public async Task<IEnumerable<RestaurantSummary>> GetAllRestaurants()
        {
            var restaurantTable = Table.LoadTable(DynamoDBClient, TableName);
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("SortKey", ScanOperator.Equal, "Summary");

            var search = restaurantTable.Scan(scanFilter);

            var results = new List<Document>();
            do
            {
                results.AddRange(await search.GetNextSetAsync());
            } while (!search.IsDone);

            return results.Select(x => RestaurantSummaryMapper.MapFromDynamoDocument(x));
        }

        public async Task<RestaurantSummary> GetRestaurantById(string id)
        {
            var restaurantTable = Table.LoadTable(DynamoDBClient, TableName);
            var restaurantDoc = await restaurantTable.GetItemAsync(id, "Summary");

            RestaurantSummary summary = null;

            if (restaurantDoc != null)
            {
                summary = RestaurantSummaryMapper.MapFromDynamoDocument(restaurantDoc);
            }

            return summary;
        }

        public async Task UpdateRestaurant(RestaurantSummary summary)
        {
            // Note: ensuring ratings aren't updated from a PUT occurs in the controller.
            // This is because this same codepath will be used to update the rating when reviews are changed.

            var document = RestaurantSummaryMapper.MapToDynamoDocument(summary);
            var restaurantTable = Table.LoadTable(DynamoDBClient, TableName);

            await restaurantTable.UpdateItemAsync(document);
        }

        public async Task DeleteRestaurant(string id)
        {
            var restaurantTable = Table.LoadTable(DynamoDBClient, TableName);
            await restaurantTable.DeleteItemAsync(id, "Summary");

            // TODO: once reviews are implemented, make sure all relevant reviews for the restaurant are also deleted.
        }
    }
}
