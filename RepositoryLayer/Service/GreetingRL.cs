using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Context;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace RepositoryLayer.Service
{
    /// <summary>
    /// Repository layer service for managing greeting messages in the database.
    /// Provides methods for adding new greetings and retrieving greeting messages by ID.
    /// </summary>
    public class GreetingRL : IGreetingRL
    {
        private readonly UserContext _Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingRL"/> class.
        /// </summary>
        /// <param name="context">The database context used for accessing greetings.</param>
        public GreetingRL(UserContext context)
        {
            _Context = context;
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
                return newGreeting;
            }
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
            var GreetingIDExist = _Context.Greetings.FirstOrDefault(e => e.Id == id);
            if (GreetingIDExist == null)
            {
                return null;
            }
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
            return _Context.Greetings.Select(g => new ResponseAllMessage
            {
                Email = g.Email,
                FirstName = g.FirstName,
                LastName = g.LastName,
                Message = g.GreetingMessage,

            }).ToList();
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
            var GreetingEntity = _Context.Greetings.FirstOrDefault(g => g.Id == requestUpdateModel.Id);
            if (GreetingEntity != null)
            {
                GreetingEntity.GreetingMessage = requestUpdateModel.Message;
                _Context.SaveChanges();
                return new ResponseAllMessage
                {
                    Email = GreetingEntity.Email,
                    FirstName = GreetingEntity.FirstName,
                    LastName = GreetingEntity.LastName,
                    Message = GreetingEntity.GreetingMessage,
                };
            }
            return null;
        }
    }
}
