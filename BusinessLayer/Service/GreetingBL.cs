using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Service;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingBL"/> class.
        /// </summary>
        /// <param name="greetingRL">Repository layer interface for greeting operations.</param>
        public GreetingBL(IGreetingRL greetingRL)
        {
            _greetingRL = greetingRL;
        }

        /// <summary>
        /// Returns the default greeting message.
        /// </summary>
        /// <returns>Returns "Hello, World!" as a default greeting.</returns>
        public string GetGreetingMessage()
        {
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
            string GreetingMessage = _greetingRL.GetGreetingByIdRL(id);
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
                return new ResponseModel<GreetingEntity>
                {
                    Success = true,
                    Message = "Greeting Message added",
                    Data = AddedOrNot
                };
            }
            return new ResponseModel<GreetingEntity>
            {
                Success = false,
                Message = "User already exists",
                Data = null
            };
        }
    }
}
