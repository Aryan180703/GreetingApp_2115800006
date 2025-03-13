using BusinessLayer.Service;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace HelloGreetingApplication.Controllers
{
    /// <summary>
    /// This class is used to create API endpoints for the Greeting API.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HelloGreetingController : ControllerBase
    {
        private readonly IGreetingBL _greetingBL;
        public HelloGreetingController(IGreetingBL greetingBL)
        {
            _greetingBL = greetingBL;
        }
        /// <summary>
        /// Generates a greeting message without storing it.
        /// </summary>
        /// <param name="FirstName">Optional first name of the user.</param>
        /// <param name="LastName">Optional last name of the user.</param>
        /// <returns>Returns a personalized greeting message.</returns>
        [HttpGet]
        public IActionResult Get()
        {

            return Ok(_greetingBL.GetGreetingMessage());
        }

        [HttpGet("GenerateGreeting/{FirstName?}/{LastName?}")]
        public IActionResult Get(string? FirstName = "", string? LastName = "")
        {
            return Ok(_greetingBL.GenerateGreeting(FirstName, LastName));
        }

        /// <summary>
        /// Generates and stores a personalized greeting message.
        /// </summary>
        /// <param name="requestGreetingModel">Request model containing user details.</param>
        /// <returns>Returns success or failure response with stored greeting data.</returns>
        [HttpPost]
        public IActionResult Post([FromBody] RequestGreetingModel requestGreetingModel)
        {
            ResponseModel<GreetingEntity> response = _greetingBL.GenerateGreetingMessage(requestGreetingModel);
            if (response.Success == true)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        /// <summary>
        /// PUT method to update the greeting message.
        /// </summary>
        /// <param name="requestModel">Request model with updated message.</param>
        /// <returns>Response model confirming the update.</returns>
        [HttpPut]
        public IActionResult Put([FromBody] RequestModel requestModel)
        {
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
            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Greeting reset to default";
            responseModel.Data = "Hello, World!";
            return Ok(responseModel);
        }
    }
}