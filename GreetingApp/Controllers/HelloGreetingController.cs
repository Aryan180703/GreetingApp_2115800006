using Microsoft.AspNetCore.Mvc;
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
        /// <summary>
        /// GET method to retrieve the greeting message.
        /// </summary>
        /// <returns>Response model with "Hello, World!"</returns>
        [HttpGet]
        public IActionResult Get()
        {
            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Hello to Greeting App API Endpoint";
            responseModel.Data = "Hello, World!";
            return Ok(responseModel);
        }

        /// <summary>
        /// POST method to receive and return a greeting message.
        /// </summary>
        /// <param name="requestModel">Request model containing a key-value pair.</param>
        /// <returns>Response model with received data.</returns>
        [HttpPost]
        public IActionResult Post([FromBody] RequestModel requestModel)
        {
            ResponseModel<string> responseModel = new ResponseModel<string>();
            responseModel.Success = true;
            responseModel.Message = "Request received successfully";
            responseModel.Data = $"Key: {requestModel.Key}, Value: {requestModel.Value}";
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