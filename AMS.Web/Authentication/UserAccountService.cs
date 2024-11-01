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

		public async Task<List<UserAccount>> GetAllUserAccounts()
		{
			var result = await _userManagementRepository.GetUsersAsync();
			return result;
		}

		public async Task<List<UserRoles>> GetAllUserRoles()
		{
			var result = await _userManagementRepository.GetRolesAsync();
			return result;
		}

		public async Task<int> UpdateLoginDates(string Id)
        {
            var result = await _userManagementRepository.UpdateLoginDates(Id);
            return result;
        }

		public async Task<int> UpdateRole(string roleId, string userId)
		{
			var result = await _userManagementRepository.UpdateRoleAsync(roleId, userId);
			return result;
		}

		public async Task<int> InsertUser(UserAccount userAccount)
		{
			var result = await _userManagementRepository.InsertUserAsync(userAccount);
			return result;
		}
        public async Task<int> UpdateUser(UserAccount userAccount)
        {
            var result = await _userManagementRepository.UpdateUserAsync(userAccount);
            return result;
        }
    }
}
