using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKEToolInterview.Models
{
    public class RestaurantSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        // TODO: as a first pass, this is just going to be a text field for hours, but this could probably be a more complex data structure
        // tied to DateTime(Offset?) in the future, to allow for better formatting and possibly timezone data.
        public string Hours { get; set; }

        public double? AverageRating { get; set; }
    }
}
