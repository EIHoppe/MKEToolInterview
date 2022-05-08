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
        private ReviewService ReviewService { get; set; }

        public RestaurantsController(RestaurantService restaurantService, ReviewService reviewService)
        {
            RestaurantService = restaurantService;
            ReviewService = reviewService;
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
                return BadRequest("Body's ID does not match the ID in the URL");
            }

            // Ensure the restaurant's rating will not be updated, by nulling it out.
            // Any updates to the rating will be triggered via any updates to its ratings.
            // Note: the mapper at present omits the AverageRating attribute entirely when mapping right now;
            //   if that changes, this code will need to update to handle that.
            summary.AverageRating = null;

            await RestaurantService.UpdateRestaurant(summary);

            return new OkResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(string id)
        {
            await RestaurantService.DeleteRestaurant(id);
            // Because the reviews would be orphaned anyway, delete all of the associated reviews when deleting a restaurant.
            await ReviewService.DeleteAllReviewsForRestaurant(id);

            return new OkResult();
        }

        [HttpPost("{restaurantId}/reviews")]
        public async Task<IActionResult> CreateReview(RestaurantReview review, string restaurantId)
        {
            var newId = await ReviewService.CreateNewReview(review, restaurantId);

            return new OkObjectResult(newId);
        }

        [HttpPut("{restaurantId}/reviews/{reviewId}")]
        public async Task<IActionResult> UpdateReview(RestaurantReview review, string restaurantId, string reviewId)
        {
            // Quick validation to make sure the body's IDs all line up correctly
            if (review.Id.ToString() != reviewId || review.RestaurantId.ToString() != restaurantId)
            {
                return BadRequest("Body's IDs do not match the IDs in the URL");
            }

            await ReviewService.UpdateReview(review);

            return new OkResult();
        }

        [HttpGet("{restaurantId}/reviews")]
        public async Task<IEnumerable<RestaurantReview>> GetReviewsForRestaurant(string restaurantId)
        {
            return await ReviewService.GetAllReviewsForRestaurant(restaurantId);
        }

        [HttpGet("{restaurantId}/reviews/{reviewId}")]
        public async Task<ActionResult<RestaurantReview>> GetReviewById(string restaurantId, string reviewId)
        {
            var review = await ReviewService.GetReviewById(restaurantId, reviewId);
            if (review == null)
            {
                return new NotFoundResult();
            }
            return review;
        }

        [HttpDelete("{restaurantId}/reviews/{reviewId}")]
        public async Task<IActionResult> DeleteReviewById(string restaurantId, string reviewId)
        {
            await ReviewService.DeleteReview(restaurantId, reviewId);

            return new OkResult();
        }
    }
}
