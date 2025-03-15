using BusinessLayer.Interface;
using BusinessLayer.Services;
using BusinessLayer.Utils;
using Microsoft.Extensions.Logging;
using ModelLayer.DTOs;
using ModelLayer.Model;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    /// <summary>
    /// Business logic layer for handling greeting messages.
    /// Implements operations related to generating, retrieving, and storing greetings.
    /// </summary>
    public class GreetingBL : IGreetingBL
    {
        private readonly IGreetingRL _greetingRL;
        private readonly TokenService _tokenService;
        private readonly ILogger<GreetingBL> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingBL"/> class.
        /// </summary>
        /// <param name="greetingRL">Repository layer interface for greeting operations.</param>
        /// <param name="logger">Logger instance for logging operations.</param>
        public GreetingBL(IGreetingRL greetingRL, TokenService tokenService, ILogger<GreetingBL> logger)
        {
            _greetingRL = greetingRL;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Returns the default greeting message.
        /// </summary>
        /// <returns>Returns "Hello, World!" as a default greeting.</returns>
        public string GetGreetingMessage()
        {
            _logger.LogInformation("GetGreetingMessage method called. Returning default greeting.");
            return "Hello World";
        }

        /// <summary>
        /// Generates a personalized greeting message based on the provided first and last name.
        /// </summary>
        /// <param name="firstName">User's first name (optional).</param>
        /// <param name="lastName">User's last name (optional).</param>
        /// <returns>Returns a response model containing the personalized greeting message.</returns>
        public ResponseModel<string> GenerateGreeting(string firstName, string lastName)
        {
            _logger.LogInformation("GenerateGreeting method called with FirstName: {FirstName}, LastName: {LastName}", firstName, lastName);

            string Message;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                Message = $"Hello, {firstName} {lastName}!";
            }
            else if (!string.IsNullOrWhiteSpace(firstName))
            {
                Message = $"Hello, {firstName}!";
            }
            else if (!string.IsNullOrWhiteSpace(lastName))
            {
                Message = $"Hello, {lastName}!";
            }
            else
            {
                Message = "Hello, World!";
            }

            _logger.LogInformation("Generated greeting message: {Message}", Message);
            return new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting Generated",
                Data = Message
            };
        }

        /// <summary>
        /// Retrieves a stored greeting message by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the stored greeting.</param>
        /// <returns>Returns a response model containing the greeting message if found; otherwise, returns an error message.</returns>
        public ResponseModel<string> GetGreetingById(int id)
        {
            _logger.LogInformation("GetGreetingById method called with ID: {Id}", id);

            string GreetingMessage = _greetingRL.GetGreetingByIdRL(id);
            if (GreetingMessage != null)
            {
                _logger.LogInformation("Greeting message found for ID: {Id}", id);
                return new ResponseModel<string>
                {
                    Success = true,
                    Message = "Greeting Message retrieved",
                    Data = GreetingMessage
                };
            }

            _logger.LogWarning("No greeting message found for ID: {Id}", id);
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Id does not exist",
                Data = GreetingMessage
            };
        }

        /// <summary>
        /// Generates and stores a personalized greeting message.
        /// </summary>
        /// <param name="requestGreetingModel">The request model containing user details.</param>
        /// <returns>Returns a response model with success status and stored greeting data.</returns>
        public ResponseModel<GreetingEntity> GenerateGreetingMessage(RequestGreetingModel requestGreetingModel)
        {
            _logger.LogInformation("GenerateGreetingMessage method called with RequestGreetingModel: {@RequestGreetingModel}", requestGreetingModel);

            string firstName = requestGreetingModel.FirstName;
            string lastName = requestGreetingModel.LastName;
            string Message;

            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                Message = $"Hello, {firstName} {lastName}!";
            }
            else if (!string.IsNullOrWhiteSpace(firstName))
            {
                Message = $"Hello, {firstName}!";
            }
            else if (!string.IsNullOrWhiteSpace(lastName))
            {
                Message = $"Hello, {lastName}!";
            }
            else
            {
                Message = "Hello, World!";
            }

            GreetingEntity AddedOrNot = _greetingRL.AddGreeting(requestGreetingModel, Message);
            if (AddedOrNot != null)
            {
                _logger.LogInformation("Greeting message added successfully for user: {FirstName} {LastName}", firstName, lastName);
                return new ResponseModel<GreetingEntity>
                {
                    Success = true,
                    Message = "Greeting Message added",
                    Data = AddedOrNot
                };
            }

            _logger.LogWarning("Failed to add greeting message. User already exists: {FirstName} {LastName}", firstName, lastName);
            return new ResponseModel<GreetingEntity>
            {
                Success = false,
                Message = "User already exists",
                Data = null
            };
        }

        /// <summary>
        /// Retrieves all greeting messages from the database and returns them wrapped in a ResponseModel.
        /// </summary>
        /// <returns>
        /// A <see cref="ResponseModel{List{ResponseAllMessage}}"/> object containing:
        /// - Success = true, Message = "All Greeting Messages retrieved", Data = List of greetings (if found).
        /// - Success = false, Message = "No Greeting Message Found", Data = null (if no greetings exist).
        /// </returns>
        public ResponseModel<List<ResponseAllMessage>> GetAllGreetingMessage()
        {
            _logger.LogInformation("GetAllGreetingMessage method called.");

            List<ResponseAllMessage> listOfGreetings = _greetingRL.GetAllGreetingMessageRL();
            if (listOfGreetings.Count == 0)
            {
                _logger.LogWarning("No greeting messages found in the database.");
                return new ResponseModel<List<ResponseAllMessage>>
                {
                    Success = false,
                    Message = "No Greeting Message Found",
                    Data = listOfGreetings
                };
            }

            _logger.LogInformation("Retrieved {Count} greeting messages from the database.", listOfGreetings.Count);
            return new ResponseModel<List<ResponseAllMessage>>
            {
                Success = true,
                Message = "All Greeting Messages retrieved",
                Data = listOfGreetings
            };
        }

        /// <summary>
        /// Updates an existing greeting message in the database if it exists.
        /// </summary>
        /// <param name="requestUpdateModel">
        /// An object containing:
        /// - <see cref="RequestUpdateModel.Id"/>: The unique identifier of the greeting message to update.
        /// - <see cref="RequestUpdateModel.Message"/>: The new greeting message text.
        /// </param>
        /// <returns>
        /// A <see cref="ResponseModel{ResponseAllMessage}"/> object containing:
        /// - **Success = true** and the updated greeting message if the update is successful.
        /// - **Success = false** if no greeting message exists for the provided ID.
        /// </returns>
        public ResponseModel<ResponseAllMessage> UpdateGreetingMessage(RequestUpdateModel requestUpdateModel)
        {
            _logger.LogInformation("UpdateGreetingMessage method called with RequestUpdateModel: {@RequestUpdateModel}", requestUpdateModel);

            ResponseAllMessage requestModel = _greetingRL.UpdateGreetingMessageRL(requestUpdateModel);
            if (requestModel == null)
            {
                _logger.LogWarning("No greeting message found for ID: {Id}", requestUpdateModel.Id);
                return new ResponseModel<ResponseAllMessage>
                {
                    Success = false,
                    Message = "No greeting message exist with the given ID",
                    Data = null
                };
            }

            _logger.LogInformation("Greeting message updated successfully for ID: {Id}", requestUpdateModel.Id);
            return new ResponseModel<ResponseAllMessage>
            {
                Success = true,
                Message = "Greeting Message Updated",
                Data = requestModel
            };
        }

        /// <summary>
        /// Deletes an existing greeting message based on the provided ID.
        /// </summary>
        /// <param name="id">The unique identifier of the greeting message to be deleted.</param>
        /// <returns>
        /// Returns a <see cref="ResponseModel{T}"/> with the following possible outcomes:
        /// - **200 OK**: If the greeting message is successfully deleted.
        /// - **404 Not Found**: If no greeting message exists at the given ID.
        /// </returns>
        public ResponseModel<ResponseAllMessage> DeleteGreeting(int id)
        {
            _logger.LogInformation("DeleteGreeting method called with ID: {Id}", id);

            ResponseAllMessage responseFromRL = _greetingRL.DeleteGreetingMessageRL(id);
            if (responseFromRL == null)
            {
                _logger.LogWarning("No greeting message found for ID: {Id}", id);
                return new ResponseModel<ResponseAllMessage>
                {
                    Success = false,
                    Message = "Greeting Message doesn't exist at given ID",
                    Data = null
                };
            }

            _logger.LogInformation("Greeting message deleted successfully for ID: {Id}", id);
            return new ResponseModel<ResponseAllMessage>
            {
                Success = true,
                Message = "Greeting Message Deleted",
                Data = responseFromRL
            };
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="user">The user details required for registration.</param>
        /// <returns>
        /// Returns a response model containing registration status:
        /// - If registration is successful, returns Success = true with user details.
        /// - If the user is already registered, returns Success = false with an appropriate message.
        /// </returns>
        public ResponseModel<ResponseRegister> RegisterBL(UserModel user)
        {
            _logger.LogInformation("User registration process started for email: {Email}", user.Email);

            user.Password = PasswordHasherService.HashPassword(user.Password);
            ResponseRegister response = _greetingRL.RegisterRL(user);

            if (response == null)
            {
                _logger.LogWarning("User registration failed. Email {Email} is already registered.", user.Email);
                return new ResponseModel<ResponseRegister>
                {
                    Success = false,
                    Message = "User already Registered",
                    Data = null
                };
            }

            _logger.LogInformation("User registration successful for email: {Email}", user.Email);
            return new ResponseModel<ResponseRegister>
            {
                Success = true,
                Message = "User Registration Successful",
                Data = response
            };
        }


        /// <summary>
        /// Handles user login by verifying credentials and returning user details if authentication is successful.
        /// </summary>
        /// <param name="login">The login details including email and password.</param>
        /// <returns>
        /// Returns a success response if authentication is successful.
        /// Returns a failure response if the email is not registered or the password is incorrect.
        /// </returns>
        public ResponseModel<ResponseRegister> LoginBL(LoginDTO login)
        {
            _logger.LogInformation("Login process started for email: {Email}", login.Email);

            UserEntity response = _greetingRL.LoginRL(login);

            if (response != null)
            {
                bool PasswordCheck = PasswordHasherService.VerifyPassword(login.Password, response.Password);
                if (PasswordCheck)
                {
                    // ✅ Generate JWT Token
                    //string token = _tokenService.GenerateToken(response);

                    _logger.LogInformation("User successfully logged in: {Email}", login.Email);
                    return new ResponseModel<ResponseRegister>
                    {
                        Success = true,
                        Message = "User Logged in",
                        Data = new ResponseRegister
                        {
                            Id = response.Id,
                            Email = response.Email,
                            FirstName = response.FirstName,
                            LastName = response.LastName,
                            //Token = token // ✅ Add Token in Response
                        }
                    };
                }

                _logger.LogWarning("Login failed - Incorrect password for email: {Email}", login.Email);
                return new ResponseModel<ResponseRegister>
                {
                    Success = false,
                    Message = "Wrong Password",
                    Data = null
                };
            }

            _logger.LogWarning("Login failed - No user registered with email: {Email}", login.Email);
            return new ResponseModel<ResponseRegister>
            {
                Success = false,
                Message = "No user registered with this Email",
                Data = null
            };
        }


    }
}