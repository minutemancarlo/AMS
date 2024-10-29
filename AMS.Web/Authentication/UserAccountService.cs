using AMS.Data.Models.Entities;
using AMS.Data.Repositories.Authentication;
namespace AMS.Web.Authentication
{
    public class UserAccountService
    {
        private readonly IUserManagementRepository _userManagementRepository;        

        public UserAccountService(IUserManagementRepository userManagementRepository)
        {
            _userManagementRepository = userManagementRepository;       
        }

        public async Task<UserAccount?> GetByUserName(string userName)
        {
            var result = await _userManagementRepository.GetUserInfoAsync(userName);
            return result;
        }

        public async Task<int> UpdateLoginDates(string Id)
        {
            var result = await _userManagementRepository.UpdateLoginDates(Id);
            return result;
        }
    }
}
