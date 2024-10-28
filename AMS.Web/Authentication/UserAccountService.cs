using AMS.Data.Models.Entities;

namespace AMS.Web.Authentication
{
    public class UserAccountService
    {
        private List<UserAccount> _users;

        public UserAccountService()
        {
            _users = new List<UserAccount>
            {
                new UserAccount{Id = Guid.NewGuid(), Name = "Carlo Boado" ,UserName = "admin",Email = "admin@admin.com" ,Password = "admin", Role = "Administrator" },
                new UserAccount{Id = Guid.NewGuid(),Name ="User User" ,UserName = "user", Email = "USER@USER.COM" ,Password = "user", Role = "User" }
            };
        }

        public UserAccount? GetByUserName(string userName)
        {
            return _users.FirstOrDefault(x => x.UserName == userName);
        }
    }
}
