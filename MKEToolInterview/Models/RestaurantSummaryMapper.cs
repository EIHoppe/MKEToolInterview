using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKEToolInterview.Models
{
    public class RestaurantSummaryMapper
    {
        // This explicitly needs to be a Dictionary and not an IDictionary to match Dynamo's API.
        public static Dictionary<string, AttributeValue> MapToDynamoAttributes(RestaurantSummary summary)
        {
            var values = new Dictionary<string, AttributeValue>();
            values["RestaurantId"] = new AttributeValue { S = summary.Id.ToString() };
            values["Name"] = new AttributeValue { S = summary.Name };
            values["Address"] = new AttributeValue { S = summary.Address };
            values["Description"] = new AttributeValue { S = summary.Description };
            values["Hours"] = new AttributeValue { S = summary.Hours };
            if (summary.AverageRating != null)
            {
                values["AverageRating"] = new AttributeValue { N = summary.AverageRating.Value.ToString() };
            }
            values["SortKey"] = new AttributeValue { S = "Summary" }; // TODO: figure out a better way to handle the SK

            return values;
        }

        public static RestaurantSummary MapFromDynamoAttributes(IDictionary<string, AttributeValue> attributeValues)
        {
            return new RestaurantSummary
            {
                Id = Guid.Parse(attributeValues["RestaurantId"].S),
                Name = attributeValues["Name"].S,
                Address = attributeValues["Address"].S,
                Description = attributeValues["Description"].S,
                Hours = attributeValues["RestaurantId"].S,
                AverageRating = double.Parse(attributeValues["RestaurantId"].N),
            };
        }
    }
}
