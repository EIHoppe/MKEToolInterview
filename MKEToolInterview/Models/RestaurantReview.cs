using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKEToolInterview.Models
{
    public class RestaurantReview
    {
        public Guid Id { get; set; }
        public double Rating { get; set; }
        public string User { get; set; }
        public string ReviewText { get; set; }
    }
}
