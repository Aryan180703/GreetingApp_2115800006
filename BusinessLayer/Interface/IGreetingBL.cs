using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IGreetingBL
    {
        public string GetGreetingMessage();
        public ResponseModel<GreetingEntity> GenerateGreetingMessage(RequestGreetingModel requestGreetingModel);
        public ResponseModel<string> GenerateGreeting(string firstName, string lastName);
    }
}
