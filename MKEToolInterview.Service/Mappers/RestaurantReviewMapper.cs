using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using MKEToolInterview.Models;
using MKEToolInterview.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MKEToolInterview.Service.Mappers
{
    public class RestaurantReviewMapper
    {
        // This explicitly needs to be a Dictionary and not an IDictionary to match Dynamo's API.
        public static Dictionary<string, AttributeValue> MapToDynamoAttributes(RestaurantReview review)
        {
            var values = new Dictionary<string, AttributeValue>();
            values["RestaurantId"] = new AttributeValue { S = review.RestaurantId.ToString() };
            values["SortKey"] = new AttributeValue { S = $"Review#{review.Id}" };
            values["Rating"] = new AttributeValue { N = review.Rating.ToString() };
            values["User"] = new AttributeValue { S = review.User };
            values["ReviewText"] = new AttributeValue { S = review.ReviewText };

            return values;
        }

        public static Document MapToDynamoDocument(RestaurantReview review)
        {
            var document = new Document();
            document["RestaurantId"] = review.RestaurantId.ToString();
            document["SortKey"] = $"Review#{review.Id}";
            document["Rating"] = review.Rating.ToString();
            document["User"] = review.User;
            document["ReviewText"] = review.ReviewText;

            return document;
        }

        public static RestaurantReview MapFromDynamoDocument(Document reviewDocument)
        {
            // We need to strip out the "Review#" part of the sort key prior to parsing
            var sortKey = DynamoDBDocumentAccessHelper.AccessDocumentAttribute(reviewDocument, "SortKey");
            var sortKeyParts = sortKey.Split("#");
            // At present, sort key only has Summary and Review#{guid}; if this changes, we can no longer assume the last part is the review's guid.
            var reviewId = sortKeyParts.Last();

            return new RestaurantReview
            {
                RestaurantId = Guid.Parse(DynamoDBDocumentAccessHelper.AccessDocumentAttribute(reviewDocument, "RestaurantId")),
                Id = Guid.Parse(reviewId),
                Rating = double.Parse(DynamoDBDocumentAccessHelper.AccessDocumentAttribute(reviewDocument, "Rating")),
                User = DynamoDBDocumentAccessHelper.AccessDocumentAttribute(reviewDocument, "User"),
                ReviewText = DynamoDBDocumentAccessHelper.AccessDocumentAttribute(reviewDocument, "ReviewText")
            };
        }
    }
}
