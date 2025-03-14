using BusinessLayer.Service;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using Microsoft.Extensions.Logging;
using HelloGreetingApplication.Controllers;
using ModelLayer.Models;
using ModelLayer.DTOs;

namespace GreetingApp.Controllers
{
    /// <summary>
    /// Controller for user-related operations such as registration, login, password management, etc.
    /// Handles HTTP requests for user management in the Greeting App.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGreetingBL _greetingBL;
        private readonly ILogger<HelloGreetingController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="greetingBL">The business logic layer interface for user operations.</param>
        /// <param name="logger">The logger for logging important events and errors.</param>
        public UserController(IGreetingBL greetingBL, ILogger<HelloGreetingController> logger)
        {
            _greetingBL = greetingBL;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="userModel">The user details required for registration.</param>
        /// <returns>
        /// Returns HTTP 200 (OK) if registration is successful.  
        /// Returns HTTP 409 (Conflict) if the user already exists.
        /// </returns>
        [HttpPost]
        public IActionResult Register(UserModel userModel)
        {
            _logger.LogInformation("Received registration request for email: {Email}", userModel.Email);

            ResponseModel<ResponseRegister> response = _greetingBL.RegisterBL(userModel);

            if (response.Success)
            {
                _logger.LogInformation("User registration successful for email: {Email}", userModel.Email);
                return Ok(response);
            }

            _logger.LogWarning("User registration failed. Email {Email} is already registered.", userModel.Email);
            return Conflict(response);
        }


        /// <summary>
        /// Handles user login by validating credentials.
        /// </summary>
        /// <param name="login">The login details containing email and password.</param>
        /// <returns>
        /// Returns a 200 OK response if login is successful.
        /// Returns a 400 Bad Request if credentials are incorrect.
        /// </returns>
        [HttpPost("login")]
        public IActionResult Login(LoginDTO login)
        {
            _logger.LogInformation("Login attempt for email: {Email}", login.Email);

            ResponseModel<ResponseRegister> response = _greetingBL.LoginBL(login);

            if (response.Success)
            {
                _logger.LogInformation("User logged in successfully: {Email}", login.Email);
                return Ok(response);
            }

            _logger.LogWarning("Invalid login attempt for email: {Email}", login.Email);
            return BadRequest(response);
        }

    }
}
