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
        public GreetingEntity AddGreeting(AddGreetingRequestModel addGreetingRequestModel);
        public string GetGreetingByIdRL(int Id , int UserId);
        public List<ResponseAllMessage> GetAllGreetingMessageRL(int UserId);
        public ResponseAllMessage UpdateGreetingMessageRL(RequestUpdateModel requestUpdateModel);
        public DeletedMessageDTO DeleteGreetingMessageRL(DeleteGreetingDTO delete);
        public ResponseRegister RegisterRL(UserModel userModel);
        public UserEntity LoginRL(LoginDTO login);
        public UserEntity GetUserByEmailRL(string email);
        public bool UpdatePasswordRL(string email, string newPassword);
        public bool UserExistOrNot(int UserId);
    }
}
