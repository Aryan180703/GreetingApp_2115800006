using BusinessLayer.Service;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace HelloGreetingApplication.Controllers
{
    /// <summary>
    /// Controller for handling Greeting API operations.
    /// Provides endpoints for generating, retrieving, updating, and deleting greetings.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HelloGreetingController : ControllerBase
    {
        private readonly IGreetingBL _greetingBL;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloGreetingController"/> class.
        /// </summary>
        /// <param name="greetingBL">Business logic layer interface for greeting operations.</param>
        public HelloGreetingController(IGreetingBL greetingBL)
        {
            _greetingBL = greetingBL;
        }

        /// <summary>
        /// Generates a default greeting message without storing it.
        /// </summary>
        /// <returns>Returns a generic greeting message.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_greetingBL.GetGreetingMessage());
        }

        /// <summary>
        /// Generates a personalized greeting message using the provided first and last name.
        /// </summary>
        /// <param name="FirstName">Optional first name of the user.</param>
        /// <param name="LastName">Optional last name of the user.</param>
        /// <returns>Returns a personalized greeting message.</returns>
        [HttpGet("GenerateGreeting/{FirstName?}/{LastName?}")]
        public IActionResult Get(string? FirstName = "", string? LastName = "")
        {
            return Ok(_greetingBL.GenerateGreeting(FirstName, LastName));
        }

        /// <summary>
        /// Retrieves a stored greeting message by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the stored greeting.</param>
        /// <returns>Returns the greeting message if found; otherwise, NotFound response.</returns>
        [HttpGet("GetGreetingByID/{Id}")]
        public IActionResult Get(int id)
        {
            ResponseModel<string> response = _greetingBL.GetGreetingById(id);
            if (response.Success)
            {
                return Ok(response);
            }
            return NotFound(response);
        }

        /// <summary>
        /// Generates and stores a personalized greeting message.
        /// </summary>
        /// <param name="requestGreetingModel">The request model containing user details.</param>
        /// <returns>Returns a response model with success status and stored greeting data.</returns>
        [HttpPost]
        public IActionResult Post([FromBody] RequestGreetingModel requestGreetingModel)
        {
            ResponseModel<GreetingEntity> response = _greetingBL.GenerateGreetingMessage(requestGreetingModel);
            if (response.Success == true)
            {
                return Ok(response);
            }
            return Conflict(response);
        }

        /// <summary>
        /// Updates an existing greeting message.
        /// </summary>
        /// <param name="requestModel">The request model containing the updated greeting message.</param>
        /// <returns>Returns a response confirming the update.</returns>
        [HttpPut]
        public IActionResult Put([FromBody] RequestModel requestModel)
        {
            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting updated successfully",
                Data = $"Updated Value: {requestModel.Value}"
            };
            return Ok(responseModel);
        }

        /// <summary>
        /// Partially updates a greeting message.
        /// </summary>
        /// <param name="requestModel">The request model containing the partial update.</param>
        /// <returns>Returns a response confirming the modification.</returns>
        [HttpPatch]
        public IActionResult Patch([FromBody] RequestModel requestModel)
        {
            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting partially updated",
                Data = $"Modified Value: {requestModel.Value}"
            };
            return Ok(responseModel);
        }

        /// <summary>
        /// Deletes or resets the greeting message to default.
        /// </summary>
        /// <returns>Returns a response confirming the reset action.</returns>
        [HttpDelete]
        public IActionResult Delete()
        {
            ResponseModel<string> responseModel = new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting reset to default",
                Data = "Hello, World!"
            };
            return Ok(responseModel);
        }
    }
}
