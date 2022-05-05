using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using MKEToolInterview.Models;
using MKEToolInterview.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKEToolInterview.Service.Mappers
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

        public static Document MapToDynamoDocument(RestaurantSummary summary)
        {
            var document = new Document();
            document["RestaurantId"] = summary.Id.ToString();
            document["Name"] = summary.Name;
            document["Address"] = summary.Address;
            document["Description"] = summary.Description;
            document["Hours"] = summary.Hours;
            if (summary.AverageRating != null)
            {
                document["AverageRating"] = summary.AverageRating.Value.ToString();
            }
            document["SortKey"] = "Summary"; // TODO: figure out a better way to handle the SK

            return document;
        }

        public static RestaurantSummary MapFromDynamoDocument(Document restaurantDocument)
        {
            // Average Rating is nullable, so handle that separately first
            var averageRatingText = DynamoDBDocumentAccessHelper.AccessDocumentAttribute(restaurantDocument, "AverageRating");
            double? averageRating = null;
            if (!string.IsNullOrEmpty(averageRatingText))
            {
                averageRating = double.Parse(averageRatingText);
            }

            return new RestaurantSummary
            {
                Id = Guid.Parse(DynamoDBDocumentAccessHelper.AccessDocumentAttribute(restaurantDocument, "RestaurantId")),
                Name = DynamoDBDocumentAccessHelper.AccessDocumentAttribute(restaurantDocument, "Name"),
                Address = DynamoDBDocumentAccessHelper.AccessDocumentAttribute(restaurantDocument, "Address"),
                Description = DynamoDBDocumentAccessHelper.AccessDocumentAttribute(restaurantDocument, "Description"),
                Hours = DynamoDBDocumentAccessHelper.AccessDocumentAttribute(restaurantDocument, "Hours"),
                AverageRating = averageRating
            };
        }
    }
}
