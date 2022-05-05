using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MKEToolInterview.Models;
using MKEToolInterview.Service;
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
        private RestaurantService RestaurantService { get; set; }

        public RestaurantsController(RestaurantService restaurantService)
        {
            RestaurantService = restaurantService;
        }

        // TODO: probably would eventually implement some sort of auth to e.g. restrict, say, deleting or updating to admin users, but likely not fitting in the first pass.

        [HttpPost]
        public async Task<IActionResult> CreateRestaurant(RestaurantSummary summary)
        {
            var newId = await RestaurantService.CreateNewRestaurant(summary);

            return new OkObjectResult(newId);
        }

        [HttpGet]
        public IEnumerable<RestaurantSummary> GetAllRestaurants()
        {
            // TODO (needed for interview version): implement retrieving summary data from dynamo

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
            // TODO (needed for interview version): implement retrieving summary data from dynamo

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
            // TODO (needed for interview version): implement update

            return new OkResult();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRestaurant(string id)
        {
            // TODO (needed for interview version): implement delete

            return new OkResult();
        }

        [HttpPost("{id}/reviews")]
        public IActionResult CreateReview(RestaurantReview review)
        {
            // TODO (needed for interview version): implement storing new review in dynamo, return ID stored

            // TODO: also have it update the average rating on the summary doc (keep a todo around because dynamo doesn't handle that great (but it is free!))

            return new OkResult();
        }

        [HttpPut("{id}/reviews/{reviewId}")]
        public IActionResult UpdateReview(string id, string reviewId)
        {
            // TODO (needed for interview version): implement updating review in dynamo
            // TODO: like create, have it update the average rating on the summary doc

            return new OkResult();
        }

        [HttpGet("{id}/reviews")]
        public IEnumerable<RestaurantReview> GetReviewsForRestaurant(string id)
        {
            // TODO (needed for interview version): implement retrieving reviews from dynamo

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
            // TODO (needed for interview version): implement retrieving reviews from dynamo

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
            // TODO (needed for interview version): implement deleting reviews from dynamo

            return new OkResult();
        }
    }
}
