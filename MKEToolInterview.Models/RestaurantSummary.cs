using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKEToolInterview.Models
{
    public class RestaurantSummary
    {
        /// <summary>
        /// Primary Partition Key for the Restaurant
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The restaurant's name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The restaurant's address. This is just a freeform text field for now; there is no separate handling of e.g. state or zip code
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// The restaurant's description
        /// </summary>
        public string Description { get; set; }

        // TODO: as a first pass, this is just going to be a text field for hours, but this could probably be a more complex data structure
        // tied to DateTime(Offset?) in the future, to allow for better formatting and possibly timezone data.
        /// <summary>
        /// The hours the restaurant is open. This is just a freeform text field for now.
        /// </summary>
        public string Hours { get; set; }

        /// <summary>
        /// The restaurant's average rating.
        ///
        /// Note: this is a calculated field, and should be treated as read-only. Any attempts to set this will be ignored.
        /// </summary>
        public double? AverageRating { get; set; }
    }
}
