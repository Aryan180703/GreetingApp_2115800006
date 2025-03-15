using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTOs;
using ModelLayer.Model;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IGreetingBL
    {
        public string GetGreetingMessage();
        public ResponseModel<GreetingEntity> GenerateGreetingMessage(RequestGreetingModel requestGreetingModel);
        public ResponseModel<string> GenerateGreeting(string firstName, string lastName);
        public ResponseModel<string> GetGreetingById(int id);
        public ResponseModel<List<ResponseAllMessage>> GetAllGreetingMessage();
        public ResponseModel<ResponseAllMessage> UpdateGreetingMessage(RequestUpdateModel requestUpdateModel);
        public ResponseModel<ResponseAllMessage> DeleteGreeting(int id);
        public ResponseModel<ResponseRegister> RegisterBL(UserModel user);
        public ResponseModel<ResponseRegister> LoginBL(LoginDTO login);
        public ResponseModel<string> ForgotPasswordBL(string email);
        public ResponseModel<string> ResetPasswordBL(string token, string newPassword);

    }
}
