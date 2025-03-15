using BusinessLayer.Service;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using Microsoft.Extensions.Logging;
using ModelLayer.DTOs;

namespace HelloGreetingApplication.Controllers
{
    /// <summary>
    /// Handles Greeting API operations such as generating, retrieving, updating, and deleting greetings.
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
        /// <param name="logger">Logger for tracking controller operations.</param>
        public HelloGreetingController(IGreetingBL greetingBL, ILogger<HelloGreetingController> logger)
        {
            _greetingBL = greetingBL;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a default greeting message without storing it.
        /// </summary>
        /// <returns>A generic greeting message with a 200 OK response.</returns>
        [HttpGet("GetDefaultMessage")]
        public IActionResult Get()
        {
            _logger.LogInformation("Retrieving default greeting message.");
            return Ok(_greetingBL.GetGreetingMessage());
        }

        /// <summary>
        /// Generates a personalized greeting message based on optional first and last names.
        /// </summary>
        /// <param name="FirstName">The first name of the user (optional, defaults to empty string).</param>
        /// <param name="LastName">The last name of the user (optional, defaults to empty string).</param>
        /// <returns>A personalized greeting message with a 200 OK response.</returns>
        [HttpGet("GenerateGreeting/{FirstName?}/{LastName?}")]
        public IActionResult Get(string? FirstName = "", string? LastName = "")
        {
            _logger.LogInformation("Generating greeting for FirstName: {FirstName}, LastName: {LastName}", FirstName, LastName);
            return Ok(_greetingBL.GenerateGreeting(FirstName, LastName));
        }

        /// <summary>
        /// Retrieves a stored greeting message by its unique identifier.
        /// </summary>
        /// <param name="getByIDDTO">DTO containing the ID of the greeting to retrieve.</param>
        /// <returns>
        /// - 200 OK with the greeting message if found.
        /// - 404 Not Found if no greeting exists for the given ID.
        /// </returns>
        [HttpGet("GetGreetingByID/{MessageId}/{UserId}")]
        public IActionResult Get(int MessageId, int UserId)
        {
            
            ResponseModel<string> response = _greetingBL.GetGreetingById(MessageId,UserId);
            if (response.Success)
            {
                
                return Ok(response);
            }

          
            return NotFound(response);
        }

        /// <summary>
        /// Retrieves all greeting messages for a specific user from the database.
        /// </summary>
        /// <param name="UserId">The ID of the user whose greetings are to be retrieved.</param>
        /// <returns>
        /// - 200 OK with a list of greeting messages if found.
        /// - 404 Not Found if no greetings are available.
        /// </returns>
        [HttpGet("GetAllGreetingMessage/{UserId}")]
        public IActionResult GetAll(int UserId)
        {
            _logger.LogInformation("Retrieving all greeting messages for UserId: {UserId}", UserId);
            ResponseModel<List<ResponseAllMessage>> response = _greetingBL.GetAllGreetingMessage(UserId);
            if (response.Success)
            {
                _logger.LogInformation("Retrieved {Count} greeting messages for UserId: {UserId}", response.Data.Count, UserId);
                return Ok(response);
            }

            _logger.LogWarning("No greeting messages found for UserId: {UserId}", UserId);
            return NotFound(response);
        }

        /// <summary>
        /// Generates and stores a personalized greeting message in the database.
        /// </summary>
        /// <param name="requestGreetingModel">The request model with user details for the greeting.</param>
        /// <returns>
        /// - 200 OK with the stored greeting data if successful.
        /// - 409 Conflict if the user already exists.
        /// </returns>
        [HttpPost("AddGreetingMessage")]
        public IActionResult Post([FromBody] AddGreetingRequestModel requestGreetingModel)
        {
            _logger.LogInformation("Adding greeting with details: {@RequestGreetingModel}", requestGreetingModel);
            ResponseModel<GreetingEntity> response = _greetingBL.AddGreetingBL(requestGreetingModel);
            if (response.Success == true)
            {
                _logger.LogInformation("Greeting added successfully");
                return Ok(response);
            }

            _logger.LogWarning("Failed to add greeting");
            return Conflict(response);
        }

        /// <summary>
        /// Updates an existing greeting message with new content.
        /// </summary>
        /// <param name="requestUpdateModel">The model containing the greeting ID and updated message.</param>
        /// <returns>
        /// - 200 OK with the updated greeting if successful.
        /// - 404 Not Found if no greeting exists for the given ID.
        /// </returns>
        [HttpPut("UpdateGreetingMessage")]
        public IActionResult Put([FromBody] RequestUpdateModel requestUpdateModel)
        {
            _logger.LogInformation("Updating greeting with details: {@RequestUpdateModel}", requestUpdateModel);
            ResponseModel<ResponseAllMessage> response = _greetingBL.UpdateGreetingMessage(requestUpdateModel);
            if (response.Success == true)
            {
                _logger.LogInformation("Greeting updated successfully for ID: {Id}", requestUpdateModel.Id);
                return Ok(response);
            }

            _logger.LogWarning("Greeting not found for ID: {Id}", requestUpdateModel.Id);
            return NotFound(response);
        }

        /// <summary>
        /// Deletes a greeting message by its ID and resets it to a default message.
        /// </summary>
        /// <param name="delete">DTO containing the ID of the greeting to delete.</param>
        /// <returns>
        /// - 200 OK with the deletion result if successful.
        /// - 404 Not Found if no greeting exists for the given ID.
        /// </returns>
        [HttpDelete("DeleteGreetingMessage")]
        public IActionResult Delete(DeleteGreetingDTO delete)
        {
            _logger.LogInformation("Deleting greeting with ID: {Id}", delete.Id);
            ResponseModel<DeletedMessageDTO> response = _greetingBL.DeleteGreeting(delete);
            if (response.Success == true)
            {
                _logger.LogInformation("Greeting deleted successfully for ID: {Id}", delete.Id);
                return Ok(response);
            }

            _logger.LogWarning("Greeting not found for ID: {Id}", delete.Id);
            return NotFound(response);
        }
    }
}