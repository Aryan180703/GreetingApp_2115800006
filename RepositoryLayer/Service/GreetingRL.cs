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
    /// Repository layer service for managing greeting messages in the database.
    /// Provides methods for adding new greetings and retrieving greeting messages by ID.
    /// </summary>
    public class GreetingRL : IGreetingRL
    {
        private readonly UserContext _Context;
        private readonly ILogger<GreetingRL> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingRL"/> class.
        /// </summary>
        /// <param name="context">The database context used for accessing greetings.</param>
        /// <param name="logger">Logger instance for logging operations.</param>
        public GreetingRL(UserContext context, ILogger<GreetingRL> logger)
        {
            _Context = context;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new greeting to the database if the user does not already exist.
        /// </summary>
        /// <param name="requestGreetingModel">The request model containing user details such as Email, FirstName, and LastName.</param>
        /// <param name="Message">The greeting message to be stored.</param>
        /// <returns>
        /// Returns the newly added <see cref="GreetingEntity"/> if the greeting is successfully added;
        /// otherwise, returns null if a greeting for the provided email already exists.
        /// </returns>
        public GreetingEntity AddGreeting(RequestGreetingModel requestGreetingModel, string Message)
        {
            _logger.LogInformation("AddGreeting method called with Email: {Email}", requestGreetingModel.Email);

            var ExistingUser = _Context.Greetings.FirstOrDefault<GreetingEntity>(e => e.Email == requestGreetingModel.Email);
            if (ExistingUser == null)
            {
                var newGreeting = new GreetingEntity
                {
                    Email = requestGreetingModel.Email,
                    FirstName = requestGreetingModel.FirstName,
                    LastName = requestGreetingModel.LastName,
                    GreetingMessage = Message
                };
                _Context.Greetings.Add(newGreeting);
                _Context.SaveChanges();

                _logger.LogInformation("Greeting added successfully for Email: {Email}", requestGreetingModel.Email);
                return newGreeting;
            }

            _logger.LogWarning("Greeting already exists for Email: {Email}", requestGreetingModel.Email);
            return null;
        }

        /// <summary>
        /// Retrieves a greeting message from the database based on the given identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the greeting message.</param>
        /// <returns>
        /// Returns the greeting message as a string if found; otherwise, returns null.
        /// </returns>
        public string GetGreetingByIdRL(int id)
        {
            _logger.LogInformation("GetGreetingByIdRL method called with ID: {Id}", id);

            var GreetingIDExist = _Context.Greetings.FirstOrDefault(e => e.Id == id);
            if (GreetingIDExist == null)
            {
                _logger.LogWarning("No greeting found for ID: {Id}", id);
                return null;
            }

            _logger.LogInformation("Greeting retrieved successfully for ID: {Id}", id);
            return GreetingIDExist.GreetingMessage;
        }

        /// <summary>
        /// Retrieves all greeting messages from the database and maps them to the ResponseAllMessage model.
        /// </summary>
        /// <returns>
        /// A list of <see cref="ResponseAllMessage"/> objects containing email, first name, last name, and the greeting message.
        /// If no greetings are found, an empty list is returned.
        /// </returns>
        public List<ResponseAllMessage> GetAllGreetingMessageRL()
        {
            _logger.LogInformation("GetAllGreetingMessageRL method called.");

            var greetings = _Context.Greetings.Select(g => new ResponseAllMessage
            {
                Email = g.Email,
                FirstName = g.FirstName,
                LastName = g.LastName,
                Message = g.GreetingMessage,
            }).ToList();

            _logger.LogInformation("Retrieved {Count} greeting messages.", greetings.Count);
            return greetings;
        }

        /// <summary>
        /// Updates an existing greeting message in the database if it exists.
        /// </summary>
        /// <param name="requestUpdateModel">An object containing the ID of the greeting message to update and the new greeting message content.</param>
        /// <returns>
        /// - Returns an updated <see cref="ResponseAllMessage"/> object if the update is successful.
        /// - Returns **null** if no greeting message is found with the given ID.
        /// </returns>
        public ResponseAllMessage UpdateGreetingMessageRL(RequestUpdateModel requestUpdateModel)
        {
            _logger.LogInformation("UpdateGreetingMessageRL method called with ID: {Id}", requestUpdateModel.Id);

            var GreetingEntity = _Context.Greetings.FirstOrDefault(g => g.Id == requestUpdateModel.Id);
            if (GreetingEntity != null)
            {
                GreetingEntity.GreetingMessage = requestUpdateModel.Message;
                _Context.SaveChanges();

                _logger.LogInformation("Greeting updated successfully for ID: {Id}", requestUpdateModel.Id);
                return new ResponseAllMessage
                {
                    Email = GreetingEntity.Email,
                    FirstName = GreetingEntity.FirstName,
                    LastName = GreetingEntity.LastName,
                    Message = GreetingEntity.GreetingMessage,
                };
            }

            _logger.LogWarning("No greeting found for ID: {Id}", requestUpdateModel.Id);
            return null;
        }

        /// <summary>
        /// Deletes a greeting message from the database if it exists.
        /// </summary>
        /// <param name="id">The unique identifier of the greeting message to be deleted.</param>
        /// <returns>
        /// Returns a <see cref="ResponseAllMessage"/> object containing the details of the deleted greeting message
        /// if it existed, otherwise returns <c>null</c>.
        /// </returns>
        public ResponseAllMessage DeleteGreetingMessageRL(int id)
        {
            _logger.LogInformation("DeleteGreetingMessageRL method called with ID: {Id}", id);

            var GreetingUser = _Context.Greetings.FirstOrDefault(e => e.Id == id);
            if (GreetingUser != null)
            {
                _Context.Remove(GreetingUser);
                _Context.SaveChanges();

                _logger.LogInformation("Greeting deleted successfully for ID: {Id}", id);
                return new ResponseAllMessage
                {
                    Email = GreetingUser.Email,
                    FirstName = GreetingUser.FirstName,
                    LastName = GreetingUser.LastName,
                    Message = GreetingUser.GreetingMessage,
                };
            }

            _logger.LogWarning("No greeting found for ID: {Id}", id);
            return null;
        }


        /// <summary>
        /// Registers a new user by checking for an existing email, 
        /// saving the user details in the database, and returning the registered user's information.
        /// Returns null if the email is already registered.
        /// </summary>
        /// <param name="userModel">The user details containing Email, FirstName, LastName, and Password.</param>
        /// <returns>
        /// A <see cref="ResponseRegister"/> object containing the registered user's Email, FirstName, and LastName.
        /// Returns null if the email is already in use.
        /// </returns>
        public ResponseRegister RegisterRL(UserModel userModel)
        {
            _logger.LogInformation("Checking if user with email {Email} already exists.", userModel.Email);
            var ExistingUser = _Context.Users.FirstOrDefault(e => e.Email == userModel.Email);

            if (ExistingUser != null)
            {
                _logger.LogWarning("User with email {Email} already exists. Registration aborted.", userModel.Email);
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
            _logger.LogInformation("User with email {Email} successfully registered.", newUser.Email);

            return new ResponseRegister
            {
                Id = newUser.Id,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
            };
        }

        /// <summary>
        /// Retrieves the user entity based on the provided email.
        /// </summary>
        /// <param name="login">The login details containing the email.</param>
        /// <returns>
        /// Returns the user entity if the email exists in the database.
        /// Returns null if no user is found with the given email.
        /// </returns>
        public UserEntity LoginRL(LoginDTO login)
        {
            _logger.LogInformation("Login request received for email: {Email}", login.Email);

            var UserExist = _Context.Users.FirstOrDefault(e => e.Email == login.Email);
            if (UserExist != null)
            {
                _logger.LogInformation("User found with email: {Email}", login.Email);
                return UserExist;
            }

            _logger.LogWarning("No user registered with email: {Email}", login.Email);
            return null;
        }

        /// <summary>
        /// Retrieves user by email for the Forgot Password feature.
        /// </summary>
        /// <param name="email">User's email to retrieve information.</param>
        /// <returns>UserEntity object if found; otherwise, null.</returns>
        public UserEntity GetUserByEmailRL(string email)
        {
            _logger.LogInformation("GetUserByEmailRL method called for email: {Email}", email);

            var user = _Context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("No user found with email: {Email}", email);
                return null;
            }

            _logger.LogInformation("User found with email: {Email}", email);
            return user;
        }

        /// <summary>
        /// Updates the password for the user if a valid JWT reset token is provided.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="newPassword">New password to be updated.</param>
        /// <returns>True if password updated successfully; otherwise, false.</returns>
        public bool UpdatePasswordRL(string email, string newPassword)
        {
            _logger.LogInformation("UpdatePasswordRL method called for email: {Email}", email);

            var user = _Context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning("No user found with email: {Email}", email);
                return false;
            }

            user.Password = newPassword; // Later, you should hash this password
            _Context.SaveChanges();

            _logger.LogInformation("Password updated successfully for email: {Email}", email);
            return true;
        }


    }
}