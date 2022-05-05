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
        public async Task<IEnumerable<RestaurantSummary>> GetAllRestaurants()
        {
            return await RestaurantService.GetAllRestaurants();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantSummary>> GetRestaurantById(string id)
        {
            var restaurant = await RestaurantService.GetRestaurantById(id);
            if (restaurant == null)
            {
                return new NotFoundResult();
            }
            return restaurant;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(RestaurantSummary summary, string id)
        {
            // Quick validation to make sure there isn't any weird things like the new version having a different ID
            if (summary.Id.ToString() != id)
            {
                return BadRequest("Body's ID does not match the id in the URL");
            }

            await RestaurantService.UpdateRestaurant(summary);

            return new OkResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(string id)
        {
            await RestaurantService.DeleteRestaurant(id);

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
