using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTOs;
using ModelLayer.Model;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IGreetingRL
    {
        public GreetingEntity AddGreeting(RequestGreetingModel requestGreetingModel, string Message);
        public string GetGreetingByIdRL(int id);
        public List<ResponseAllMessage> GetAllGreetingMessageRL();
        public ResponseAllMessage UpdateGreetingMessageRL(RequestUpdateModel requestUpdateModel);
        public ResponseAllMessage DeleteGreetingMessageRL(int id);
        public ResponseRegister RegisterRL(UserModel userModel);
        public UserEntity LoginRL(LoginDTO login);
        public UserEntity GetUserByEmailRL(string email);
        public bool UpdatePasswordRL(string email, string newPassword);
    }
}
