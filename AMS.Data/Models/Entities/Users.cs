using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Data.Models.Entities
{
    public class UserAccount
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class UserSession
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class UserRoles
    {
        public int RoleId { get; set; } = 0;
        public string? RoleName { get; set; }
    }
}
