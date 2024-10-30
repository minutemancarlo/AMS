using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Data.Models.Entities
{
    public class UserAccount : LoginInfo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }    
        public string? Role { get; set; }   
        public bool isActive { get; set; }
    }

    public class UserSession : LoginInfo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class LoginInfo
    {
        public DateTime? CurrentLoginDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

    public class UserRoles : BaseModel
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
