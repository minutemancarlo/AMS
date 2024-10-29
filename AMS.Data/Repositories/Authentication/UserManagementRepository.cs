using AMS.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Blazor.SubtleCrypto;
using Microsoft.AspNetCore.Components;
using AMS.Data.Utilities;

namespace AMS.Data.Repositories.Authentication
{

    public interface IUserManagementRepository
    {
        Task<UserAccount?> GetUserInfoAsync(string userName);
        Task<int> UpdateLoginDates(string Id);
    }
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly DateTimeHelper _dateTimeHelper;

        public UserManagementRepository(IDbConnection dbConnection, DateTimeHelper dateTimeHelper)
        {
            _dbConnection = dbConnection;
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<UserAccount?> GetUserInfoAsync(string userName)
        {
            var query = "SELECT a.Id, a.Name, a.Username, a.Email, a.Password, a.CurrentLoginDate, a.LastLoginDate, a.isActive, c.Name as Role FROM Users a inner join UserRoles b on a.Id=b.UserId inner join Roles c on b.RoleId=c.Id WHERE UserName = @UserName";
            var user = await _dbConnection.QueryFirstOrDefaultAsync<UserAccount>(query, new { UserName = userName });

            if (user != null)
            {
                user.CurrentLoginDate = _dateTimeHelper.ConvertUtcToAppTimeZone(user.CurrentLoginDate);
                user.LastLoginDate = _dateTimeHelper.ConvertUtcToAppTimeZone(user.LastLoginDate);
            }

            return user;
        }


        public async Task<int> UpdateLoginDates(string Id)
        {
            var dateToday = _dateTimeHelper.GetCurrentUtc();
            var query = "Update Users set LastLoginDate = CurrentLoginDate, CurrentLoginDate = @DateToday where Id = @ID";
            return await _dbConnection.ExecuteScalarAsync<int>(query, new { DateToday = dateToday, ID = Id });
        }
    }
}
