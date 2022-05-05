using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MKEToolInterview.Service.Helpers
{
    public class DynamoDBDocumentAccessHelper
    {
        public static string AccessDocumentAttribute(Document document, string attributeName)
        {
            var attributes = document.GetAttributeNames();
            string returnedString = null;

            if (attributes.Contains(attributeName))
            {
                var value = document[attributeName];
                if (value is Primitive)
                {
                    returnedString = value.AsPrimitive().Value.ToString();
                }
                // TODO: add list handling (out of scope for first pass)
            }

            return returnedString;
        }
    }
}
