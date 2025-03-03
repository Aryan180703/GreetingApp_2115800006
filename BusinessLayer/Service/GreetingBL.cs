using System;
using BusinessLayer.Interface;

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
            return "Hello, World!";
        }

        /// <summary>
        /// Generates a personalized greeting message.
        /// </summary>
        /// <param name="firstName">User's first name (optional).</param>
        /// <param name="lastName">User's last name (optional).</param>
        /// <returns>Personalized greeting message.</returns>
        public string GenerateGreetingMessage(string firstName, string lastName)
        {
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                return $"Hello, {firstName} {lastName}!";
            }
            else if (!string.IsNullOrWhiteSpace(firstName))
            {
                return $"Hello, {firstName}!";
            }
            else if (!string.IsNullOrWhiteSpace(lastName))
            {
                return $"Hello, {lastName}!";
            }
            else
            {
                return "Hello, World!";
            }
        }
    }
}
