using BusinessLayer.Service;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<HelloGreetingController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelloGreetingController"/> class.
        /// </summary>
        /// <param name="greetingBL">Business logic layer interface for greeting operations.</param>
        /// <param name="logger">Logger instance for logging operations.</param>
        public HelloGreetingController(IGreetingBL greetingBL, ILogger<HelloGreetingController> logger)
        {
            _greetingBL = greetingBL;
            _logger = logger;
        }

        /// <summary>
        /// Generates a default greeting message without storing it.
        /// </summary>
        /// <returns>Returns a generic greeting message.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Get method called to retrieve default greeting message.");
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
            _logger.LogInformation("GenerateGreeting method called with FirstName: {FirstName}, LastName: {LastName}", FirstName, LastName);
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
            _logger.LogInformation("GetGreetingByID method called with ID: {Id}", id);
            ResponseModel<string> response = _greetingBL.GetGreetingById(id);
            if (response.Success)
            {
                _logger.LogInformation("Greeting message found for ID: {Id}", id);
                return Ok(response);
            }

            _logger.LogWarning("No greeting message found for ID: {Id}", id);
            return NotFound(response);
        }

        /// <summary>
        /// Retrieves all greeting messages from the database and returns an HTTP response.
        /// </summary>
        /// <returns>
        /// - **200 OK** with a response model containing greeting messages if found.
        /// - **404 Not Found** if no greetings are available.
        /// </returns>
        [HttpGet("GetAllGreetingMessage")]
        public IActionResult GetAll()
        {
            _logger.LogInformation("GetAllGreetingMessage method called to retrieve all greetings.");
            ResponseModel<List<ResponseAllMessage>> response = _greetingBL.GetAllGreetingMessage();
            if (response.Success)
            {
                _logger.LogInformation("Retrieved {Count} greeting messages.", response.Data.Count);
                return Ok(response);
            }

            _logger.LogWarning("No greeting messages found in the database.");
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
            _logger.LogInformation("Post method called with RequestGreetingModel: {@RequestGreetingModel}", requestGreetingModel);
            ResponseModel<GreetingEntity> response = _greetingBL.GenerateGreetingMessage(requestGreetingModel);
            if (response.Success == true)
            {
                _logger.LogInformation("Greeting message added successfully for user: {FirstName} {LastName}", requestGreetingModel.FirstName, requestGreetingModel.LastName);
                return Ok(response);
            }

            _logger.LogWarning("Failed to add greeting message. User already exists: {Email}", requestGreetingModel.Email);
            return Conflict(response);
        }

        /// <summary>
        /// Updates an existing greeting message and returns the updated response.
        /// </summary>
        /// <param name="requestUpdateModel">
        /// An object containing the unique identifier of the greeting message and the new message content.
        /// </param>
        /// <returns>
        /// A <see cref="ResponseModel{ResponseAllMessage}"/>:
        /// - If successful, returns a response with <c>Success = true</c> and the updated message.
        /// - If no greeting message exists for the given ID, returns <c>Success = false</c> with an appropriate message.
        /// </returns>
        [HttpPut()]
        public IActionResult Put([FromBody] RequestUpdateModel requestUpdateModel)
        {
            _logger.LogInformation("Put method called with RequestUpdateModel: {@RequestUpdateModel}", requestUpdateModel);
            ResponseModel<ResponseAllMessage> response = _greetingBL.UpdateGreetingMessage(requestUpdateModel);
            if (response.Success == true)
            {
                _logger.LogInformation("Greeting message updated successfully for ID: {Id}", requestUpdateModel.Id);
                return Ok(response);
            }

            _logger.LogWarning("No greeting message found for ID: {Id}", requestUpdateModel.Id);
            return NotFound(response);
        }

        /// <summary>
        /// Deletes an existing greeting message by its unique identifier and resets it to a default message.
        /// </summary>
        /// <param name="Id">The unique identifier of the greeting message to be deleted.</param>
        /// <returns>
        /// Returns a <see cref="IActionResult"/> containing a <see cref="ResponseModel{T}"/> with the following outcomes:
        /// - **200 OK**: If the greeting message is successfully deleted and updated.
        /// - **404 Not Found**: If no greeting message exists with the given ID.
        /// </returns>
        [HttpDelete("DeleteGreetingMessage/{Id}")]
        public IActionResult Delete(int Id)
        {
            _logger.LogInformation("Delete method called with ID: {Id}", Id);
            ResponseModel<ResponseAllMessage> response = _greetingBL.DeleteGreeting(Id);
            if (response.Success == true)
            {
                _logger.LogInformation("Greeting message deleted successfully for ID: {Id}", Id);
                return Ok(response);
            }

            _logger.LogWarning("No greeting message found for ID: {Id}", Id);
            return NotFound(response);
        }
    }
}