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

namespace AMS.Data.Repositories.Authentication
{
    
    public interface IUserManagementRepository
    {
        Task<UserAccount?> GetUserInfoAsync(string userName);
    }
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserManagementRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<UserAccount?> GetUserInfoAsync(string userName)
        {
            var query = "SELECT a.Id, a.Name, a.Username, a.Email, a.Password, c.Name as Role FROM Users a inner join UserRoles b on a.Id=b.UserId inner join Roles c on b.RoleId=c.Id WHERE UserName = @UserName";
            var user = await _dbConnection.QueryFirstOrDefaultAsync<UserAccount>(query, new { UserName = userName });
            return user;
        }
    }
}
