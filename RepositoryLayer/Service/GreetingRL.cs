using Microsoft.Extensions.Logging;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Context;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using ModelLayer.DTOs;

namespace RepositoryLayer.Service
{
    /// <summary>
    /// Manages database operations for greeting messages and user authentication.
    /// </summary>
    public class GreetingRL : IGreetingRL
    {
        private readonly UserContext _Context;
        private readonly ILogger<GreetingRL> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingRL"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing greetings and users.</param>
        /// <param name="logger">Logger for tracking repository operations.</param>
        public GreetingRL(UserContext context, ILogger<GreetingRL> logger)
        {
            _Context = context;
            _logger = logger;
        }


        public bool UserExistOrNot(int UserId)
        {
            var User = _Context.Users.FirstOrDefault(e => e.Id == UserId);
            if (User == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds a new greeting to the database if it doesn’t already exist for the user.
        /// </summary>
        /// <param name="addGreetingRequestModel">The request model with user ID and greeting message.</param>
        /// <returns>
        /// The newly added <see cref="GreetingEntity"/> if successful; otherwise, null if a duplicate exists.
        /// </returns>
        /// 
        public GreetingEntity AddGreeting(AddGreetingRequestModel addGreetingRequestModel)
        {
            _logger.LogInformation("Processing add greeting request for UserId: {UserId}", addGreetingRequestModel.UserID);
            var ExistingGreeting = _Context.Greetings.FirstOrDefault<GreetingEntity>(e => e.UserId == addGreetingRequestModel.UserID && e.GreetingMessage == addGreetingRequestModel.Message);

            if (ExistingGreeting == null)
            {
                var newGreeting = new GreetingEntity
                {
                    UserId = addGreetingRequestModel.UserID,
                    GreetingMessage = addGreetingRequestModel.Message,
                };
                _Context.Greetings.Add(newGreeting);
                _Context.SaveChanges();

                _logger.LogInformation("Greeting added successfully for UserId: {UserId}", addGreetingRequestModel.UserID);
                return new GreetingEntity
                {
                    Id = newGreeting.Id,
                    UserId = newGreeting.UserId,
                    GreetingMessage = newGreeting.GreetingMessage
                };
            }

            _logger.LogWarning("Greeting already exists for UserId: {UserId}", addGreetingRequestModel.UserID);
            return null;
        }

        /// <summary>
        /// Retrieves a greeting message by its unique identifier and user ID.
        /// </summary>
        /// <param name="getByIDDTO">DTO containing the greeting ID and user ID.</param>
        /// <returns>
        /// The greeting message as a string if found; otherwise, null.
        /// </returns>
        public string GetGreetingByIdRL(int Id , int UserId)
        {
            _logger.LogInformation("Retrieving greeting for ID: {Id} and UserId: {UserId}", Id, UserId);
            var GreetingExist = _Context.Greetings.FirstOrDefault(e => e.Id == Id && e.UserId == UserId);

            if (GreetingExist == null)
            {
                _logger.LogWarning("No greeting found for ID: {Id} and UserId: {UserId}", Id, UserId);
                return null;
            }

            _logger.LogInformation("Greeting retrieved successfully for ID: {Id}", Id);
            return GreetingExist.GreetingMessage;
        }

        /// <summary>
        /// Retrieves all greeting messages for a specific user from the database.
        /// </summary>
        /// <param name="userId">The ID of the user whose greetings are to be retrieved.</param>
        /// <returns>
        /// A list of <see cref="ResponseAllMessage"/> objects; empty if no greetings are found.
        /// </returns>
        public List<ResponseAllMessage> GetAllGreetingMessageRL(int userId)
        {
            _logger.LogInformation("Retrieving all greetings for UserId: {UserId}", userId);
            var greetings = _Context.Greetings
                .Where(g => g.UserId == userId)
                .Select(g => new ResponseAllMessage
                {
                    Id = g.Id,
                    Message = g.GreetingMessage,
                })
                .ToList();

            _logger.LogInformation("Retrieved {Count} greetings for UserId: {UserId}", greetings.Count, userId);
            return greetings;
        }

        /// <summary>
        /// Updates an existing greeting message if it exists for the given ID and user.
        /// </summary>
        /// <param name="requestUpdateModel">The model containing the greeting ID, user ID, and new message.</param>
        /// <returns>
        /// An updated <see cref="ResponseAllMessage"/> if successful; otherwise, null.
        /// </returns>
        public ResponseAllMessage UpdateGreetingMessageRL(RequestUpdateModel requestUpdateModel)
        {
            _logger.LogInformation("Updating greeting for ID: {Id} and UserId: {UserId}", requestUpdateModel.Id, requestUpdateModel.UserID);
            var GreetingEntity = _Context.Greetings.FirstOrDefault(g => g.Id == requestUpdateModel.Id && g.UserId == requestUpdateModel.UserID);

            if (GreetingEntity != null)
            {
                GreetingEntity.GreetingMessage = requestUpdateModel.Message;
                _Context.SaveChanges();

                _logger.LogInformation("Greeting updated successfully for ID: {Id}", requestUpdateModel.Id);
                return new ResponseAllMessage
                {
                    Id = requestUpdateModel.Id,
                    Message = GreetingEntity.GreetingMessage,
                };
            }

            _logger.LogWarning("No greeting found for ID: {Id} and UserId: {UserId}", requestUpdateModel.Id, requestUpdateModel.UserID);
            return null;
        }

        /// <summary>
        /// Deletes a greeting message from the database if it exists.
        /// </summary>
        /// <param name="delete">DTO containing the greeting ID and user ID to delete.</param>
        /// <returns>
        /// A <see cref="DeletedMessageDTO"/> with the deleted message if successful; otherwise, null.
        /// </returns>
        public DeletedMessageDTO DeleteGreetingMessageRL(DeleteGreetingDTO delete)
        {
            _logger.LogInformation("Deleting greeting for ID: {Id} and UserId: {UserId}", delete.Id, delete.UserId);
            var GreetingUser = _Context.Greetings.FirstOrDefault(e => e.Id == delete.Id && e.UserId == delete.UserId);

            if (GreetingUser != null)
            {
                _Context.Remove(GreetingUser);
                _Context.SaveChanges();

                _logger.LogInformation("Greeting deleted successfully for ID: {Id}", delete.Id);
                return new DeletedMessageDTO
                {
                    DeletedMessage = GreetingUser.GreetingMessage,
                };
            }

            _logger.LogWarning("No greeting found for ID: {Id} and UserId: {UserId}", delete.Id, delete.UserId);
            return null;
        }

        /// <summary>
        /// Registers a new user if the email is not already in use.
        /// </summary>
        /// <param name="userModel">The user details (email, first name, last name, password).</param>
        /// <returns>
        /// A <see cref="ResponseRegister"/> with user details if successful; otherwise, null.
        /// </returns>
        public ResponseRegister RegisterRL(UserModel userModel)
        {
            _logger.LogInformation("Processing registration for email: {Email}", userModel.Email);
            var ExistingUser = _Context.Users.FirstOrDefault(e => e.Email == userModel.Email);

            if (ExistingUser != null)
            {
                _logger.LogWarning("User already exists with email: {Email}", userModel.Email);
                return null;
            }

            var newUser = new UserEntity
            {
                Email = userModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Password = userModel.Password,
            };

            _Context.Users.Add(newUser);
            _Context.SaveChanges();
            _logger.LogInformation("User registered successfully with email: {Email}", newUser.Email);

            return new ResponseRegister
            {
                Id = newUser.Id,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
            };
        }

        /// <summary>
        /// Retrieves a user entity by email for login purposes.
        /// </summary>
        /// <param name="login">The login details containing the email.</param>
        /// <returns>
        /// The <see cref="UserEntity"/> if found; otherwise, null.
        /// </returns>
        public UserEntity LoginRL(LoginDTO login)
        {
            _logger.LogInformation("Processing login for email: {Email}", login.Email);
            var UserExist = _Context.Users.FirstOrDefault(e => e.Email == login.Email);

            if (UserExist != null)
            {
                _logger.LogInformation("User found for email: {Email}", login.Email);
                return UserExist;
            }

            _logger.LogWarning("No user found for email: {Email}", login.Email);
            return null;
        }

        /// <summary>
        /// Retrieves a user by email for password reset purposes.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>
        /// The <see cref="UserEntity"/> if found; otherwise, null.
        /// </returns>
        public UserEntity GetUserByEmailRL(string email)
        {
            _logger.LogInformation("Retrieving user by email: {Email}", email);
            var user = _Context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("No user found for email: {Email}", email);
                return null;
            }

            _logger.LogInformation("User retrieved successfully for email: {Email}", email);
            return user;
        }

        /// <summary>
        /// Updates a user's password in the database.
        /// </summary>
        /// <param name="email">The email of the user whose password is to be updated.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>
        /// True if the password is updated successfully; otherwise, false.
        /// </returns>
        public bool UpdatePasswordRL(string email, string newPassword)
        {
            _logger.LogInformation("Processing password update for email: {Email}", email);
            var user = _Context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("No user found for email: {Email}", email);
                return false;
            }

            user.Password = newPassword;
            _Context.SaveChanges();

            _logger.LogInformation("Password updated successfully for email: {Email}", email);
            return true;
        }
    }
}