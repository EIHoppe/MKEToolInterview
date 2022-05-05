using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MKEToolInterview.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKEToolInterview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        // TODO: probably should implement some sort of auth to e.g. restrict, say, deleting or updating to admin users

        [HttpPost]
        public IActionResult CreateRestaurant(RestaurantSummary summary)
        {
            // TODO: implement storing summary data in dynamo, return ID stored

            return new OkResult();
        }

        [HttpGet]
        public IEnumerable<RestaurantSummary> GetAllRestaurants()
        {
            // TODO: implement retrieving summary data from dynamo

            return new[]
            {
                new RestaurantSummary
                {
                    Name = "Restaurante Firste",
                    Address = "123 Fake Street Milwaukee, WI 53202",
                    Description = "It is a restaurant. It is also first.",
                    AverageRating = 4.3,
                    Hours = "Sun-Thurs 11-9, Fri-Sat 11-11"
                },
                new RestaurantSummary
                {
                    Name = "Restaurant The Second",
                    Address = "338 Other Street Milwaukee, WI 53205",
                    Description = "It is a different restaurant. It is second I suppose.",
                    AverageRating = 3.8,
                    Hours = "Sun-Sat 12-8"
                }
            };
        }

        [HttpGet("{id}")]
        public RestaurantSummary GetRestaurantById(string id)
        {
            // TODO: implement retrieving summary data from dynamo

            return new RestaurantSummary
            {
                Name = "Restaurante Firste",
                Address = "123 Fake Street Milwaukee, WI 53202",
                Description = "It is a restaurant. It is also first.",
                AverageRating = 4.3,
                Hours = "Sun-Thurs 11-9, Fri-Sat 11-11"
            };
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRestaurant(RestaurantSummary summary, string id)
        {
            // TODO: implement update

            return new OkResult();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRestaurant(string id)
        {
            // TODO: implement delete

            return new OkResult();
        }

        [HttpPost("{id}/reviews")]
        public IActionResult CreateReview(RestaurantReview review)
        {
            // TODO: implement storing new review in dynamo, return ID stored

            // TODO: also have it update the average rating on the summary doc (keep a todo around because dynamo doesn't handle that great (but it is free!))

            return new OkResult();
        }

        [HttpPut("{id}/reviews/{reviewId}")]
        public IActionResult UpdateReview(string id, string reviewId)
        {
            // TODO: implement updating review in dynamo
            // TODO: like create, have it update the average rating on the summary doc

            return new OkResult();
        }

        [HttpGet("{id}/reviews")]
        public IEnumerable<RestaurantReview> GetReviewsForRestaurant(string id)
        {
            // TODO: implement retrieving reviews from dynamo

            return new[] {
                new RestaurantReview
                {
                    User = "Some Person",
                    ReviewText = "This restaurant was ok.",
                    Rating = 3
                },
                new RestaurantReview
                {
                    User = "Some Other Person",
                    ReviewText = "This restaurant was lovely, I had a good meal.",
                    Rating = 4.5
                }
            };
        }

        [HttpGet("{id}/reviews/{reviewId}")]
        public RestaurantReview GetReviewById(string id, string reviewId)
        {
            // TODO: implement retrieving reviews from dynamo

            return new RestaurantReview
            {
                User = "Some Person",
                ReviewText = "This restaurant was ok.",
                Rating = 3
            };
        }

        [HttpDelete("{id}/reviews/{reviewId}")]
        public IActionResult DeleteReviewById(string id, string reviewId)
        {
            // TODO: implement deleting reviews from dynamo

            return new OkResult();
        }
    }
}
