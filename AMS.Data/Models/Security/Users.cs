using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Data.Models.Security
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }        
    }

    public record LogggedInUserModel(int Id, string Username, string Email)
    {
        public Claim[] ToClaims() =>
            [
                new Claim(ClaimTypes.NameIdentifier,Id.ToString()),
                new Claim(ClaimTypes.Name, Username),
                new Claim(ClaimTypes.Email, Email)
            ];
    }

}
