using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MKEToolInterview.Models;
using MKEToolInterview.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKEToolInterview.Test
{
    [TestClass]
    public class ReviewServiceTests
    {
        private RestaurantService restaurantService;
        private ReviewService reviewService;
        private AmazonDynamoDBClient dynamoClient;
        private const string tableName = "mketool-restaurants-test";
        private Guid restaurantId;

        [TestInitialize]
        public async Task InitializeTests()
        {
            dynamoClient = new AmazonDynamoDBClient();
            restaurantService = new RestaurantService(dynamoClient, tableName);
            reviewService = new ReviewService(restaurantService, dynamoClient, tableName);

            await CleanupDatabase();

            await CreateInitialTestRestaurant();
        }

        [TestCleanup]
        public async Task CleanupAfterTests()
        {
            await CleanupDatabase();
        }

        private async Task CleanupDatabase()
        {
            var restaurantTable = Table.LoadTable(dynamoClient, tableName);
            var scanFilter = new ScanFilter();
            scanFilter.AddCondition("SortKey", ScanOperator.Equal, "Summary");

            var search = restaurantTable.Scan(scanFilter);

            var results = new List<Document>();
            do
            {
                results.AddRange(await search.GetNextSetAsync());
            } while (!search.IsDone);

            foreach (var document in results)
            {
                await restaurantTable.DeleteItemAsync(document);
            }
        }

        private async Task CreateInitialTestRestaurant()
        {
            var testRestaurant = new RestaurantSummary
            {
                Name = "Test Restaurant",
                Address = "123 Fake Street",
                Hours = "24/7",
                Description = "This is a test and such"
            };

            restaurantId = await restaurantService.CreateNewRestaurant(testRestaurant);
        }

        [TestMethod]
        public async Task ReviewService_CanAddAReviewToARestaurant()
        {
            // Arrange
            var review = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 2.5
            };

            // Act
            var reviewId = await reviewService.CreateNewReview(review, restaurantId.ToString());

            // Assert
            var actualReviews = await reviewService.GetAllReviewsForRestaurant(restaurantId.ToString());

            Assert.AreEqual(1, actualReviews.Count());
            var actualReview = actualReviews.First();
            Assert.AreEqual(review.User, actualReview.User);
            Assert.AreEqual(review.ReviewText, actualReview.ReviewText);
            Assert.AreEqual(review.Rating, actualReview.Rating);
            Assert.AreEqual(reviewId, actualReview.Id);
        }

        [TestMethod]
        public async Task ReviewService_LimitsRatingsToTheRangeOf0To5WhenCreating()
        {
            // Arrange
            var highReview = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 5.5
            };
            var negativeReview = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = -2.5
            };

            // Act
            var highReviewId = await reviewService.CreateNewReview(highReview, restaurantId.ToString());
            var negativeReviewId = await reviewService.CreateNewReview(negativeReview, restaurantId.ToString());

            // Assert
            var actualReviews = await reviewService.GetAllReviewsForRestaurant(restaurantId.ToString());

            Assert.AreEqual(2, actualReviews.Count());
            var actualHighReview = actualReviews.First(x => x.Id == highReviewId);
            var actualNegativeReview = actualReviews.First(x => x.Id == negativeReviewId);
            Assert.AreEqual(5.0, actualHighReview.Rating);
            Assert.AreEqual(0.0, actualNegativeReview.Rating);
        }

        [TestMethod]
        public async Task ReviewService_CanUpdateAnExistingReview()
        {
            // Arrange
            var review = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 2.5
            };

            var reviewId = await reviewService.CreateNewReview(review, restaurantId.ToString());

            var updatedReview = new RestaurantReview
            {
                Id = reviewId.Value,
                RestaurantId = restaurantId,
                User = "Test User Updated",
                ReviewText = "blah blah blah but different",
                Rating = 3.2
            };

            // Act
            await reviewService.UpdateReview(updatedReview);

            // Assert
            var actualReviews = await reviewService.GetAllReviewsForRestaurant(restaurantId.ToString());

            Assert.AreEqual(1, actualReviews.Count());
            var actualReview = actualReviews.First();
            Assert.AreEqual(updatedReview.User, actualReview.User);
            Assert.AreEqual(updatedReview.ReviewText, actualReview.ReviewText);
            Assert.AreEqual(updatedReview.Rating, actualReview.Rating);
            Assert.AreEqual(reviewId, actualReview.Id);
        }

        [TestMethod]
        public async Task ReviewService_LimitsRatingsToTheRangeOf0To5WhenUpdating()
        {
            // Arrange
            var soonToBeHighReview = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 3.5
            };
            var soonToBeNegativeReview = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 3.5
            };

            var highReviewId = await reviewService.CreateNewReview(soonToBeHighReview, restaurantId.ToString());
            var negativeReviewId = await reviewService.CreateNewReview(soonToBeNegativeReview, restaurantId.ToString());

            var highReview = new RestaurantReview
            {
                Id = highReviewId.Value,
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 6.5
            };
            var negativeReview = new RestaurantReview
            {
                Id = negativeReviewId.Value,
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = -3.5
            };

            // Act
            await reviewService.UpdateReview(highReview);
            await reviewService.UpdateReview(negativeReview);

            // Assert
            var actualReviews = await reviewService.GetAllReviewsForRestaurant(restaurantId.ToString());

            Assert.AreEqual(2, actualReviews.Count());
            var actualHighReview = actualReviews.First(x => x.Id == highReviewId);
            var actualNegativeReview = actualReviews.First(x => x.Id == negativeReviewId);
            Assert.AreEqual(5.0, actualHighReview.Rating);
            Assert.AreEqual(0.0, actualNegativeReview.Rating);
        }

        [TestMethod]
        public async Task ReviewService_CanDeleteAnExistingReview()
        {
            // Arrange
            var review1 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 2.5
            };
            var review2 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 2.5
            };

            var reviewId1 = await reviewService.CreateNewReview(review1, restaurantId.ToString());
            var reviewId2 = await reviewService.CreateNewReview(review2, restaurantId.ToString());

            // Act
            await reviewService.DeleteReview(restaurantId.ToString(), reviewId2.Value.ToString());

            // Assert
            var actualReviews = await reviewService.GetAllReviewsForRestaurant(restaurantId.ToString());

            Assert.AreEqual(1, actualReviews.Count());
            Assert.AreEqual(reviewId1, actualReviews.First().Id);
        }

        [TestMethod]
        public async Task ReviewService_CanDeleteAllReviewsForOneRestaurant()
        {
            // Arrange
            var review1 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 2.5
            };
            var review2 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 2.5
            };

            var reviewId1 = await reviewService.CreateNewReview(review1, restaurantId.ToString());
            var reviewId2 = await reviewService.CreateNewReview(review2, restaurantId.ToString());

            // Act
            await reviewService.DeleteAllReviewsForRestaurant(restaurantId.ToString());

            // Assert
            var actualReviews = await reviewService.GetAllReviewsForRestaurant(restaurantId.ToString());

            Assert.IsFalse(actualReviews.Any());
        }

        [TestMethod]
        public async Task ReviewService_UpdatesTheAverageRatingOnARestaurantWhenReviewIsCreated()
        {
            // Arrange
            var review1 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 2
            };
            var review2 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 4
            };

            await reviewService.CreateNewReview(review1, restaurantId.ToString());

            // Act
            await reviewService.CreateNewReview(review2, restaurantId.ToString());

            // Assert
            var restaurant = await restaurantService.GetRestaurantById(restaurantId.ToString());

            Assert.AreEqual(3, restaurant.AverageRating);
        }

        [TestMethod]
        public async Task ReviewService_UpdatesTheAverageRatingOnARestaurantWhenReviewIsUpdated()
        {
            // Arrange
            var review1 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 2
            };
            var review2 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 4
            };

            await reviewService.CreateNewReview(review1, restaurantId.ToString());
            var reviewId2 = await reviewService.CreateNewReview(review2, restaurantId.ToString());

            var updatedReview2 = new RestaurantReview
            {
                Id = reviewId2.Value,
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 5
            };

            // Act
            await reviewService.UpdateReview(updatedReview2);

            // Assert
            var restaurant = await restaurantService.GetRestaurantById(restaurantId.ToString());

            Assert.AreEqual(3.5, restaurant.AverageRating);
        }

        [TestMethod]
        public async Task ReviewService_UpdatesTheAverageRatingOnARestaurantWhenReviewIsDeleted()
        {
            // Arrange
            var review1 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 2
            };
            var review2 = new RestaurantReview
            {
                RestaurantId = restaurantId,
                User = "Test User",
                ReviewText = "blah blah blah",
                Rating = 4
            };

            await reviewService.CreateNewReview(review1, restaurantId.ToString());
            var reviewId2 = await reviewService.CreateNewReview(review2, restaurantId.ToString());

            // Act
            await reviewService.DeleteReview(restaurantId.ToString(), reviewId2.Value.ToString());

            // Assert
            var restaurant = await restaurantService.GetRestaurantById(restaurantId.ToString());

            Assert.AreEqual(2, restaurant.AverageRating);
        }
    }
}
