using System.Security.Claims;
using BusinessLayer.Interface;
using BusinessLayer.Services;
using BusinessLayer.Utils;
using Email.Interface;
using Microsoft.Extensions.Logging;
using ModelLayer.DTOs;
using ModelLayer.Model;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    /// <summary>
    /// Manages business logic for greeting operations and user authentication.
    /// </summary>
    public class GreetingBL : IGreetingBL
    {
        private readonly IGreetingRL _greetingRL;
        private readonly ITokenService _tokenService;
        private readonly ILogger<GreetingBL> _logger;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingBL"/> class.
        /// </summary>
        /// <param name="greetingRL">Repository layer interface for greeting operations.</param>
        /// <param name="tokenService">Service for generating and validating tokens.</param>
        /// <param name="logger">Logger for tracking business logic operations.</param>
        /// <param name="emailService">Service for sending emails.</param>
        public GreetingBL(IGreetingRL greetingRL, ITokenService tokenService, ILogger<GreetingBL> logger, IEmailService emailService)
        {
            _emailService = emailService;
            _greetingRL = greetingRL;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a default greeting message.
        /// </summary>
        /// <returns>The string "Hello, World!" as the default greeting.</returns>
        public string GetGreetingMessage()
        {
            _logger.LogInformation("Retrieving default greeting message.");
            return "Hello World";
        }

        /// <summary>
        /// Generates a personalized greeting message using the provided first and last names.
        /// </summary>
        /// <param name="firstName">The user's first name (optional).</param>
        /// <param name="lastName">The user's last name (optional).</param>
        /// <returns>A response model with the generated greeting message.</returns>
        public ResponseModel<string> GenerateGreeting(string firstName, string lastName)
        {
            _logger.LogInformation("Generating greeting for FirstName: {FirstName}, LastName: {LastName}", firstName, lastName);

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
        /// <param name="getByIDDTO">DTO containing the ID of the greeting to retrieve.</param>
        /// <returns>
        /// A response model with the greeting message if found; otherwise, an error message.
        /// </returns>
        public ResponseModel<string> GetGreetingById(int Id , int UserId)
        {
            bool UserExist = _greetingRL.UserExistOrNot(UserId);
            if (!UserExist)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "User with given UserID is not registered",
                    Data = null
                };
            }
            string GreetingMessage = _greetingRL.GetGreetingByIdRL(Id , UserId);

            if (GreetingMessage != null)
            {
                
                return new ResponseModel<string>
                {
                    Success = true,
                    Message = "Greeting Message retrieved",
                    Data = GreetingMessage
                };
            }

            
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Greeting for provided Id does not exist",
                Data = GreetingMessage
            };
        }

        /// <summary>
        /// Adds a new personalized greeting message to the database.
        /// </summary>
        /// <param name="addGreetingRequestModel">The request model with user details and greeting data.</param>
        /// <returns>
        /// A response model with the stored greeting entity if successful; otherwise, an error message.
        /// </returns>
        public ResponseModel<GreetingEntity> AddGreetingBL(AddGreetingRequestModel addGreetingRequestModel)
        {
            bool UserExist = _greetingRL.UserExistOrNot(addGreetingRequestModel.UserID);

            if (UserExist)
            {
                _logger.LogInformation("Adding greeting with details: {AddGreetingRequestModel}", addGreetingRequestModel);
                GreetingEntity AddedOrNot = _greetingRL.AddGreeting(addGreetingRequestModel);

                if (AddedOrNot != null)
                {
                    _logger.LogInformation("Greeting added successfully for UserId: {UserId}", AddedOrNot.UserId);
                    return new ResponseModel<GreetingEntity>
                    {
                        Success = true,
                        Message = "Greeting Message added",
                        Data = AddedOrNot
                    };
                }

                _logger.LogWarning("Failed to add greeting. Duplicate message exists for UserId: {UserId}", addGreetingRequestModel.UserID);
                return new ResponseModel<GreetingEntity>
                {
                    Success = false,
                    Message = "Exactly Same Message with this user ID already exists",
                    Data = null
                };
            }
            return new ResponseModel<GreetingEntity>
            {
                Success = false,
                Message = "User with given UserID is not registered",
                Data = null
            };
            
        }

        /// <summary>
        /// Retrieves all greeting messages for a specific user from the database.
        /// </summary>
        /// <param name="UserId">The ID of the user whose greetings are to be retrieved.</param>
        /// <returns>
        /// A response model with a list of greeting messages if found; otherwise, an empty list with a failure message.
        /// </returns>
        public ResponseModel<List<ResponseAllMessage>> GetAllGreetingMessage(int UserId)
        {
            bool UserExist = _greetingRL.UserExistOrNot(UserId);
            if (!UserExist)
            {
                return new ResponseModel<List<ResponseAllMessage>>
                {
                    Success = false,
                    Message = "User with given UserID is not registered",
                    Data = null
                };
            }
            _logger.LogInformation("Retrieving all greetings for UserId: {UserId}", UserId);
            List<ResponseAllMessage> listOfGreetings = _greetingRL.GetAllGreetingMessageRL(UserId);

            if (listOfGreetings.Count == 0)
            {
                _logger.LogWarning("No greetings found for UserId: {UserId}", UserId);
                return new ResponseModel<List<ResponseAllMessage>>
                {
                    Success = false,
                    Message = "No Greeting Message Found",
                    Data = listOfGreetings
                };
            }

            _logger.LogInformation("Retrieved {Count} greetings for UserId: {UserId}", listOfGreetings.Count, UserId);
            return new ResponseModel<List<ResponseAllMessage>>
            {
                Success = true,
                Message = "All Greeting Messages retrieved for user id provided",
                Data = listOfGreetings
            };
        }

        /// <summary>
        /// Updates an existing greeting message with new content.
        /// </summary>
        /// <param name="requestUpdateModel">The model containing the greeting ID and updated message.</param>
        /// <returns>
        /// A response model with the updated greeting if successful; otherwise, an error message.
        /// </returns>
        public ResponseModel<ResponseAllMessage> UpdateGreetingMessage(RequestUpdateModel requestUpdateModel)
        {
            bool UserExist = _greetingRL.UserExistOrNot(requestUpdateModel.UserID);
            if (!UserExist)
            {
                return new ResponseModel<ResponseAllMessage>
                {
                    Success = false,
                    Message = "User with given UserID is not registered",
                    Data = null
                };
            }
            _logger.LogInformation("Updating greeting with details: {@RequestUpdateModel}", requestUpdateModel);
            ResponseAllMessage requestModel = _greetingRL.UpdateGreetingMessageRL(requestUpdateModel);

            if (requestModel == null)
            {
                _logger.LogWarning("Greeting not found for ID: {Id}", requestUpdateModel.Id);
                return new ResponseModel<ResponseAllMessage>
                {
                    Success = false,
                    Message = "Greeting Message not found",
                    Data = null
                };
            }

            _logger.LogInformation("Greeting updated successfully for ID: {Id}", requestUpdateModel.Id);
            return new ResponseModel<ResponseAllMessage>
            {
                Success = true,
                Message = "Greeting Message Updated",
                Data = requestModel
            };
        }

        /// <summary>
        /// Deletes a greeting message by its ID and resets it to a default state.
        /// </summary>
        /// <param name="delete">DTO containing the ID of the greeting to delete.</param>
        /// <returns>
        /// A response model with deletion details if successful; otherwise, an error message.
        /// </returns>
        public ResponseModel<DeletedMessageDTO> DeleteGreeting(DeleteGreetingDTO delete)
        {
            bool UserExist = _greetingRL.UserExistOrNot(delete.UserId);
            if (!UserExist)
            {
                return new ResponseModel<DeletedMessageDTO>
                {
                    Success = false,
                    Message = "User with given UserID is not registered",
                    Data = null
                };
            }
            _logger.LogInformation("Deleting greeting with ID: {Id}", delete.Id);
            DeletedMessageDTO responseFromRL = _greetingRL.DeleteGreetingMessageRL(delete);

            if (responseFromRL == null)
            {
                _logger.LogWarning("Greeting not found for ID: {Id}", delete.Id);
                return new ResponseModel<DeletedMessageDTO>
                {
                    Success = false,
                    Message = "Greeting Message doesn't exist for given Message ID",
                    Data = null
                };
            }

            _logger.LogInformation("Greeting deleted successfully for ID: {Id}", delete.Id);
            return new ResponseModel<DeletedMessageDTO>
            {
                Success = true,
                Message = "Greeting Message Deleted",
                Data = responseFromRL
            };
        }

        /// <summary>
        /// Registers a new user with hashed password and stores their details.
        /// </summary>
        /// <param name="user">The user details for registration (e.g., email, password).</param>
        /// <returns>
        /// A response model with user details if successful; otherwise, an error message if the user already exists.
        /// </returns>
        public ResponseModel<ResponseRegister> RegisterBL(UserModel user)
        {
            _logger.LogInformation("Processing registration for email: {Email}", user.Email);
            user.Password = PasswordHasherService.HashPassword(user.Password);
            ResponseRegister response = _greetingRL.RegisterRL(user);

            if (response == null)
            {
                _logger.LogWarning("Registration failed. Email already registered: {Email}", user.Email);
                return new ResponseModel<ResponseRegister>
                {
                    Success = false,
                    Message = "User already Registered",
                    Data = null
                };
            }

            _logger.LogInformation("User registered successfully for email: {Email}", user.Email);
            return new ResponseModel<ResponseRegister>
            {
                Success = true,
                Message = "User Registration Successful",
                Data = response
            };
        }

        /// <summary>
        /// Authenticates a user by verifying their email and password.
        /// </summary>
        /// <param name="login">The login details (email and password).</param>
        /// <returns>
        /// A response model with user details if login succeeds; otherwise, an error message.
        /// </returns>
        public ResponseModel<ResponseRegister> LoginBL(LoginDTO login)
        {
            _logger.LogInformation("Processing login for email: {Email}", login.Email);
            UserEntity response = _greetingRL.LoginRL(login);

            if (response != null)
            {
                bool PasswordCheck = PasswordHasherService.VerifyPassword(login.Password, response.Password);
                if (PasswordCheck)
                {
                    _logger.LogInformation("Login successful for email: {Email}", login.Email);
                    return new ResponseModel<ResponseRegister>
                    {
                        Success = true,
                        Message = "User Logged in",
                        Data = new ResponseRegister
                        {
                            Id = response.Id,
                            Email = response.Email,
                            FirstName = response.FirstName,
                            LastName = response.LastName
                        }
                    };
                }

                _logger.LogWarning("Login failed. Incorrect password for email: {Email}", login.Email);
                return new ResponseModel<ResponseRegister>
                {
                    Success = false,
                    Message = "Wrong Password",
                    Data = null
                };
            }

            _logger.LogWarning("Login failed. No user found for email: {Email}", login.Email);
            return new ResponseModel<ResponseRegister>
            {
                Success = false,
                Message = "No user registered with this Email",
                Data = null
            };
        }

        /// <summary>
        /// Initiates a password reset by sending a reset link to the user's email.
        /// </summary>
        /// <param name="email">The email address of the user requesting a password reset.</param>
        /// <returns>
        /// A response model indicating success if the reset link is sent; otherwise, an error message.
        /// </returns>
        public ResponseModel<string> ForgotPasswordBL(string email)
        {
            _logger.LogInformation("Processing forgot password request for email: {Email}", email);
            var user = _greetingRL.GetUserByEmailRL(email);

            if (user == null)
            {
                _logger.LogWarning("No user found for email: {Email}", email);
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "User not found",
                    Data = null
                };
            }

            string resetToken = _tokenService.GenerateToken(user.Id, user.Email);
            string subject = "Password Reset Request";
            string message = $"Click the link to reset your password: https://yourapp.com/reset-password?token={resetToken}";
            _emailService.SendEmail(email, subject, message);

            _logger.LogInformation("Password reset link sent to email: {Email}", email);
            return new ResponseModel<string>
            {
                Success = true,
                Message = "Password reset link sent to email",
                Data = resetToken // Optional: Consider removing for security
            };
        }

        /// <summary>
        /// Resets a user's password using a token and new password.
        /// </summary>
        /// <param name="token">The reset token sent to the user's email.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>
        /// A response model indicating success if the password is updated; otherwise, an error message.
        /// </returns>
        public ResponseModel<string> ResetPasswordBL(string token, string newPassword)
        {
            _logger.LogInformation("Processing password reset with token: {Token}", token);
            var tokenData = _tokenService.ValidateToken(token);

            if (tokenData == null)
            {
                _logger.LogWarning("Invalid or expired token: {Token}", token);
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid or expired token",
                    Data = null
                };
            }

            var emailClaim = tokenData.FindFirst(ClaimTypes.Email) ?? tokenData.FindFirst("Email");
            if (emailClaim == null)
            {
                _logger.LogWarning("No email claim found in token: {Token}", token);
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid token structure",
                    Data = null
                };
            }

            string email = emailClaim.Value;
            newPassword = PasswordHasherService.HashPassword(newPassword);
            bool isUpdated = _greetingRL.UpdatePasswordRL(email, newPassword);

            if (!isUpdated)
            {
                _logger.LogError("Failed to update password for email: {Email}", email);
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Failed to update password",
                    Data = null
                };
            }

            _logger.LogInformation("Password reset successfully for email: {Email}", email);
            return new ResponseModel<string>
            {
                Success = true,
                Message = "Password updated successfully",
                Data = null
            };
        }
    }
}