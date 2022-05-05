using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MKEToolInterview.Models;
using MKEToolInterview.Service.Mappers;
using System;
using System.Collections.Generic;
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
    }
}
