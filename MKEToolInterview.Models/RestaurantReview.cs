using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKEToolInterview.Models
{
    public class RestaurantReview
    {

        /// <summary>
        /// The ID of the restaurant this review is associated with
        /// </summary>
        public Guid RestaurantId { get; set; }

        /// <summary>
        /// The review's ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The rating awarded to the restaurant.
        ///
        /// Note: this is pinned from a 0-5 scale; any ratings beyond that will be Max()/Min()'d to the appropriate scale.
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// The name of the person who left the review
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The body of the review
        /// </summary>
        public string ReviewText { get; set; }
    }
}
