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
    public class GreetingBL : IGreetingBL
    {
        IGreetingRL _greetingRL;
        public GreetingBL(IGreetingRL greetingRL)
        {
            _greetingRL = greetingRL;
        }
        /// <summary>
        /// Returns the default greeting message.
        /// </summary>
        /// <returns>"Hello, World!"</returns>
        public string GetGreetingMessage()
        {
            return "Hello World";
        }

        public ResponseModel<string> GenerateGreeting(string firstName , string lastName)
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
        /// Generates a personalized greeting message.
        /// </summary>
        /// <param name="firstName">User's first name (optional).</param>
        /// <param name="lastName">User's last name (optional).</param>
        /// <returns>Personalized greeting message.</returns>
        public ResponseModel<GreetingEntity> GenerateGreetingMessage(RequestGreetingModel requestGreetingModel)
        {
            string firstName = requestGreetingModel.FirstName;
            string lastName = requestGreetingModel.LastName;
            string Message;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                Message =  $"Hello, {firstName} {lastName}!";
            }
            else if (!string.IsNullOrWhiteSpace(firstName))
            {
                Message =  $"Hello, {firstName}!";
            }
            else if (!string.IsNullOrWhiteSpace(lastName))
            {
                Message =  $"Hello, {lastName}!";
            }
            else
            {
                Message =  "Hello, World!";
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
                Message = "User already Exist",
                Data = null
            };

        }
    }
}   


