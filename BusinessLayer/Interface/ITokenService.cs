using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface ITokenService
    {
        public string GenerateToken(string email);
        public string GenerateToken(int userId, string email);
        public ClaimsPrincipal ValidateToken(string token);
    }
}
