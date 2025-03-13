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
    public class GreetingRL : IGreetingRL
    {
        UserContext _Context;

        public GreetingRL(UserContext context)
        {
            _Context = context;
        }
        public GreetingEntity AddGreeting(RequestGreetingModel requestGreetingModel, string Message)
        {
            var ExistingUser = _Context.Greetings.FirstOrDefault<GreetingEntity>(e =>  e.Email == requestGreetingModel.Email);
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
    }
}
