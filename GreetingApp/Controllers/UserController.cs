using BusinessLayer.Service;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using Microsoft.Extensions.Logging;
using HelloGreetingApplication.Controllers;
using ModelLayer.Models;
using ModelLayer.DTOs;
using BusinessLayer.Services;

namespace GreetingApp.Controllers
{
    /// <summary>
    /// Manages user-related operations including registration, login, and password management.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IGreetingBL _greetingBL;
        private readonly ILogger<UserController> _logger; // Adjusted to UserController (assuming typo in original)

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="greetingBL">Business logic layer interface for user operations.</param>
        /// <param name="tokenService">Service for generating authentication tokens.</param>
        /// <param name="logger">Logger for tracking user-related events and errors.</param>
        public UserController(IGreetingBL greetingBL, ITokenService tokenService, ILogger<UserController> logger)
        {
            _tokenService = tokenService;
            _greetingBL = greetingBL;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user in the system with the provided details.
        /// </summary>
        /// <param name="userModel">The user details required for registration (e.g., email, password).</param>
        /// <returns>
        /// - 200 OK with registration details if successful.
        /// - 409 Conflict if the email is already registered.
        /// </returns>
        [HttpPost]
        public IActionResult Register(UserModel userModel)
        {
            _logger.LogInformation("Processing registration request for email: {Email}", userModel.Email);
            ResponseModel<ResponseRegister> response = _greetingBL.RegisterBL(userModel);

            if (response.Success)
            {
                _logger.LogInformation("User registered successfully for email: {Email}", userModel.Email);
                return Ok(response);
            }

            _logger.LogWarning("Registration failed. Email already in use: {Email}", userModel.Email);
            return Conflict(response);
        }

        /// <summary>
        /// Authenticates a user and generates a token upon successful login.
        /// </summary>
        /// <param name="login">The login details containing email and password.</param>
        /// <returns>
        /// - 200 OK with a token and response data if login succeeds.
        /// - 401 Unauthorized if credentials are invalid.
        /// </returns>
        [HttpPost("login")]
        public IActionResult Login(LoginDTO login)
        {
            _logger.LogInformation("Processing login attempt for email: {Email}", login.Email);
            ResponseModel<ResponseRegister> response = _greetingBL.LoginBL(login);

            if (response.Success)
            {
                string token = _tokenService.GenerateToken(response.Data.Email);
                _logger.LogInformation("Login successful for email: {Email}. Token generated.", login.Email);
                return Ok(new { Token = token, response });
            }

            _logger.LogWarning("Login failed due to invalid credentials for email: {Email}", login.Email);
            return Unauthorized(new { Message = "Invalid Credentials" });
        }

        /// <summary>
        /// Initiates a password reset by sending a reset link to the user's email.
        /// </summary>
        /// <param name="request">DTO containing the user's email address.</param>
        /// <returns>
        /// - 200 OK if the reset link is sent successfully.
        /// - 400 Bad Request if the email is not found or invalid.
        /// </returns>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDTO request)
        {
            _logger.LogInformation("Processing forgot password request for email: {Email}", request.Email);
            var response = _greetingBL.ForgotPasswordBL(request.Email);

            if (response.Success)
            {
                _logger.LogInformation("Password reset link sent successfully to email: {Email}", request.Email);
                return Ok(response);
            }

            _logger.LogWarning("Failed to send reset link. Email not found: {Email}", request.Email);
            return BadRequest(response);
        }

        /// <summary>
        /// Resets the user's password using a provided token and new password.
        /// </summary>
        /// <param name="request">DTO containing the reset token and new password.</param>
        /// <returns>
        /// - 200 OK if the password is reset successfully.
        /// - 400 Bad Request if the token is invalid or expired.
        /// </returns>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO request)
        {
            _logger.LogInformation("Processing password reset request with token: {Token}", request.Token);
            var response = _greetingBL.ResetPasswordBL(request.Token, request.NewPassword);

            if (response.Success)
            {
                _logger.LogInformation("Password reset successful for token: {Token}", request.Token);
                return Ok(response);
            }

            _logger.LogWarning("Password reset failed. Invalid or expired token: {Token}", request.Token);
            return BadRequest(response);
        }
    }
}