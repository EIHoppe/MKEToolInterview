using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MKEToolInterview.Models;
using MKEToolInterview.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKEToolInterview.Test
{
    [TestClass]
    public class RestaurantServiceTests
    {
        private RestaurantService restaurantService;
        private AmazonDynamoDBClient dynamoClient;
        private const string tableName = "mketool-restaurants-test";

        [TestInitialize]
        public async Task InitializeTests()
        {
            dynamoClient = new AmazonDynamoDBClient();
            restaurantService = new RestaurantService(dynamoClient, tableName);

            await CleanupDatabase();
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

        [TestMethod]
        public async Task RestaurantService_CanCreateRestaurants()
        {
            // Arrange
            var testRestaurant = new RestaurantSummary
            {
                Name = "Test Restaurant",
                Address = "123 Fake Street",
                Hours = "24/7",
                Description = "This is a test and such"
            };

            // Act
            var id = await restaurantService.CreateNewRestaurant(testRestaurant);

            // Assert
            var actualRestaurant = await restaurantService.GetRestaurantById(id.ToString());

            Assert.AreEqual(testRestaurant.Name, actualRestaurant.Name);
            Assert.AreEqual(testRestaurant.Address, actualRestaurant.Address);
            Assert.AreEqual(testRestaurant.Hours, actualRestaurant.Hours);
            Assert.AreEqual(testRestaurant.Description, actualRestaurant.Description);
        }

        [TestMethod]
        public async Task RestaurantService_IgnoresAverageRatingOnModelWhenCreatingRestaurant()
        {
            // Arrange
            var testRestaurant = new RestaurantSummary
            {
                Name = "Test Restaurant",
                Address = "123 Fake Street",
                Hours = "24/7",
                Description = "This is a test and such",
                AverageRating = 4.8
            };

            // Act
            var id = await restaurantService.CreateNewRestaurant(testRestaurant);

            // Assert
            var actualRestaurant = await restaurantService.GetRestaurantById(id.ToString());

            Assert.IsNull(actualRestaurant.AverageRating);
        }

        [TestMethod]
        public async Task RestaurantService_CanUpdateExistingRestaurants()
        {
            // Arrange
            var testRestaurant = new RestaurantSummary
            {
                Name = "Test Restaurant",
                Address = "123 Fake Street",
                Hours = "24/7",
                Description = "This is a test and such"
            };

            var id = await restaurantService.CreateNewRestaurant(testRestaurant);

            var updatedRestaurant = new RestaurantSummary
            {
                Id = id,
                Name = "Test Restaurant but Updated",
                Address = "456 Other Street",
                Hours = "only an hour a day and what hour it is is random",
                Description = "Wow it's changed!"
            };

            // Act
            await restaurantService.UpdateRestaurant(updatedRestaurant);

            // Assert
            var actualRestaurant = await restaurantService.GetRestaurantById(id.ToString());

            Assert.AreEqual(updatedRestaurant.Name, actualRestaurant.Name);
            Assert.AreEqual(updatedRestaurant.Address, actualRestaurant.Address);
            Assert.AreEqual(updatedRestaurant.Hours, actualRestaurant.Hours);
            Assert.AreEqual(updatedRestaurant.Description, actualRestaurant.Description);
        }

        [TestMethod]
        public async Task RestaurantService_SetsAverageRatingOnModelWhenUpdatingRestaurant()
        {
            // Arrange
            var testRestaurant = new RestaurantSummary
            {
                Name = "Test Restaurant",
                Address = "123 Fake Street",
                Hours = "24/7",
                Description = "This is a test and such"
            };

            var id = await restaurantService.CreateNewRestaurant(testRestaurant);

            var updatedRestaurant = new RestaurantSummary
            {
                Id = id,
                Name = "Test Restaurant but Updated",
                Address = "456 Other Street",
                Hours = "only an hour a day and what hour it is is random",
                Description = "Wow it's changed!",
                AverageRating = 4.9
            };

            // Act
            await restaurantService.UpdateRestaurant(updatedRestaurant);

            // Assert
            var actualRestaurant = await restaurantService.GetRestaurantById(id.ToString());

            Assert.AreEqual(updatedRestaurant.AverageRating, actualRestaurant.AverageRating);
        }

        [TestMethod]
        public async Task RestaurantService_CanDeleteExistingRestaurants()
        {
            // Arrange
            var testRestaurant1 = new RestaurantSummary
            {
                Name = "Test Restaurant",
                Address = "123 Fake Street",
                Hours = "24/7",
                Description = "This is a test and such"
            };

            var testRestaurant2 = new RestaurantSummary
            {
                Name = "Test Restaurant but Different",
                Address = "999 Second Street",
                Hours = "10-8",
                Description = "It's a second test"
            };

            var id1 = await restaurantService.CreateNewRestaurant(testRestaurant1);
            var id2 = await restaurantService.CreateNewRestaurant(testRestaurant2);

            // Act
            await restaurantService.DeleteRestaurant(id2.ToString());

            // Assert
            var restaurants = await restaurantService.GetAllRestaurants();

            Assert.AreEqual(1, restaurants.Count());
            Assert.AreEqual(id1, restaurants.First().Id);
        }


    }
}
