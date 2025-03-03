using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.Model;

namespace HelloGreetingApplication.Controllers
{
    /// <summary>
    /// This class is used to create API endpoints for the Greeting API.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HelloGreetingController : ControllerBase
    {
        private readonly ILogger<HelloGreetingController> _logger;
        private readonly IGreetingBL _greetingBL;

        /// <summary>
        /// Constructor to initialize the logger.
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public HelloGreetingController(IGreetingBL greetingBL, ILogger<HelloGreetingController> logger)
        {
            _greetingBL = greetingBL;
            _logger = logger;
        }

        /// <summary>
        /// GET method to retrieve the greeting message.
        /// </summary>
        /// <returns>Response model with "Hello, World!"</returns>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("GET request received at HelloGreetingController");

            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Hello to Greeting App API Endpoint";
            responseModel.Data = "Hello, World!";
            return Ok(responseModel);
        }

        /// <summary>
        /// POST method to generate a personalized greeting message.
        /// </summary>
        /// <param name="requestModel">Request model containing user attributes.</param>
        /// <returns>Response model with personalized greeting.</returns>
        [HttpPost]
        public IActionResult Post([FromBody] RequestModel requestModel)
        {
            _logger.LogInformation("POST request received with FirstName: {FirstName}, LastName: {LastName}",
                requestModel.FirstName, requestModel.LastName);

            // Call Business Layer to generate greeting
            string greetingMessage = _greetingBL.GenerateGreetingMessage(requestModel.FirstName, requestModel.LastName);

            _logger.LogInformation("Generated Greeting: {GreetingMessage}", greetingMessage);

            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Personalized Greeting Generated";
            responseModel.Data = greetingMessage;
            return Ok(responseModel);
        }


        /// <summary>
        /// PUT method to update the greeting message.
        /// </summary>
        /// <param name="requestModel">Request model with updated message.</param>
        /// <returns>Response model confirming the update.</returns>
        [HttpPut]
        public IActionResult Put([FromBody] RequestModel requestModel)
        {
            _logger.LogInformation("PUT request received with Key: {Key}, Updated Value: {Value}", 
                requestModel.Key, requestModel.Value);

            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Greeting updated successfully";
            responseModel.Data = $"Updated Value: {requestModel.Value}";
            return Ok(responseModel);
        }

        /// <summary>
        /// PATCH method to modify the greeting message partially.
        /// </summary>
        /// <param name="requestModel">Request model containing partial update.</param>
        /// <returns>Response model confirming the modification.</returns>
        [HttpPatch]
        public IActionResult Patch([FromBody] RequestModel requestModel)
        {
            _logger.LogInformation("PATCH request received with Key: {Key}, Modified Value: {Value}", 
                requestModel.Key, requestModel.Value);

            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Greeting partially updated";
            responseModel.Data = $"Modified Value: {requestModel.Value}";
            return Ok(responseModel);
        }

        /// <summary>
        /// DELETE method to reset the greeting message.
        /// </summary>
        /// <returns>Response model confirming the reset action.</returns>
        [HttpDelete]
        public IActionResult Delete()
        {
            _logger.LogWarning("DELETE request received. Greeting reset to default.");

            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Greeting reset to default";
            responseModel.Data = "Hello, World!";
            return Ok(responseModel);
        }
    }
}
