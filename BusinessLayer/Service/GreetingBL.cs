using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ModelLayer.Model;
namespace BusinessLayer.Service
{
    public class GreetingBL : IGreetingBL
    {
        /// <summary>
        /// Returns the default greeting message.
        /// </summary>
        /// <returns>"Hello, World!"</returns>
        public string GetGreetingMessage()
        {
            return "Hello World";
        }
        /// <summary>
        /// Generates a personalized greeting message.
        /// </summary>
        /// <param name="firstName">User's first name (optional).</param>
        /// <param name="lastName">User's last name (optional).</param>
        /// <returns>Personalized greeting message.</returns>
        public ResponseModel<string> GenerateGreetingMessage(RequestModel requestModel)
        {
            string firstName = requestModel.FirstName;
            string lastName = requestModel.LastName;
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
            return new ResponseModel<string>
            {
                Success = true,
                Message = "Greeting Message Generated",
                Data = Message
            };


        }
    }
}   


