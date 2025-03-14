using BusinessLayer.Interface;
using Microsoft.Extensions.Logging;
using ModelLayer.Model;
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
        private readonly ILogger<GreetingBL> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingBL"/> class.
        /// </summary>
        /// <param name="greetingRL">Repository layer interface for greeting operations.</param>
        /// <param name="logger">Logger instance for logging operations.</param>
        public GreetingBL(IGreetingRL greetingRL, ILogger<GreetingBL> logger)
        {
            _greetingRL = greetingRL;
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
    }
}