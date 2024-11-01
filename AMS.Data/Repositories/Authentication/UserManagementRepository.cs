using AMS.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using AMS.Data.Utilities;

namespace AMS.Data.Repositories.Authentication
{
	public interface IUserManagementRepository
	{
		Task<UserAccount?> GetUserInfoAsync(string userName);
		Task<List<UserAccount>> GetUsersAsync();
		Task<List<UserRoles>> GetRolesAsync();
		Task<int> UpdateLoginDates(string Id);
		Task<int> InsertUserAsync(UserAccount userAccount);
		Task<bool> UserNameExistsAsync(string userName);
		Task<bool> UserNameExistsAsync(string userName, string userId);
		Task<bool> EmailExistsAsync(string email);
		Task<bool> EmailExistsAsync(string email, string userId);
		Task<bool> PhoneExistsAsync(string phone);
		Task<bool> PhoneExistsAsync(string phone, string userId);
		Task<int> UpdateRoleAsync(string roleId, string userId);
		Task<int> UpdateUserAsync(UserAccount userAccount);


    }

	public class UserManagementRepository : IUserManagementRepository
	{
		private readonly Func<IDbConnection> _connectionFactory;
		private readonly DateTimeHelper _dateTimeHelper;

		public UserManagementRepository(Func<IDbConnection> connectionFactory, DateTimeHelper dateTimeHelper)
		{
			_connectionFactory = connectionFactory;
			_dateTimeHelper = dateTimeHelper;
		}

		public async Task<UserAccount?> GetUserInfoAsync(string userName)
		{
			var query = "SELECT a.Id, a.Name, a.Username, a.Email, a.Password, a.CurrentLoginDate, a.LastLoginDate, a.isActive, c.Name as Role " +
						"FROM Users a " +
						"INNER JOIN UserRoles b ON a.Id = b.UserId " +
						"INNER JOIN Roles c ON b.RoleId = c.Id " +
						"WHERE UserName = @UserName";

			using (var connection = _connectionFactory())
			{
				connection.Open();
				var user = await connection.QueryFirstOrDefaultAsync<UserAccount>(query, new { UserName = userName });

				if (user != null)
				{
					user.CurrentLoginDate = _dateTimeHelper.ConvertUtcToAppTimeZone(user.CurrentLoginDate);
					user.LastLoginDate = _dateTimeHelper.ConvertUtcToAppTimeZone(user.LastLoginDate);
				}

				return user;
			}
		}

		public async Task<List<UserAccount>> GetUsersAsync()
		{
			var query = "SELECT a.Id, a.Name, a.Username, a.Email, a.Phone, a.Password, a.CurrentLoginDate, a.LastLoginDate, a.isActive, c.Name as Role " +
						"FROM Users a " +
						"LEFT JOIN UserRoles b ON a.Id = b.UserId " +
						"LEFT JOIN Roles c ON b.RoleId = c.Id";

			using (var connection = _connectionFactory())
			{
				connection.Open();
				var users = (await connection.QueryAsync<UserAccount>(query)).ToList();

				foreach (var user in users)
				{
					user.CurrentLoginDate = _dateTimeHelper.ConvertUtcToAppTimeZone(user.CurrentLoginDate);
					user.LastLoginDate = _dateTimeHelper.ConvertUtcToAppTimeZone(user.LastLoginDate);
				}

				return users;
			}
		}

		public async Task<List<UserRoles>> GetRolesAsync()
		{
			var query = "SELECT Id as RoleId, Name as RoleName, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn FROM Roles";

			using (var connection = _connectionFactory())
			{
				connection.Open();
				var roles = (await connection.QueryAsync<UserRoles>(query)).ToList();

				foreach (var role in roles)
				{
					role.CreatedOn = _dateTimeHelper.ConvertUtcToAppTimeZone(role.CreatedOn);
					role.UpdatedOn = _dateTimeHelper.ConvertUtcToAppTimeZone(role.UpdatedOn);
				}

				return roles;
			}
		}

		public async Task<int> UpdateLoginDates(string Id)
		{
			var dateToday = _dateTimeHelper.GetCurrentUtc();
			var query = "UPDATE Users SET LastLoginDate = CurrentLoginDate, CurrentLoginDate = @DateToday WHERE Id = @ID";

			using (var connection = _connectionFactory())
			{
				connection.Open();

				using (var transaction = connection.BeginTransaction())
				{
					try
					{
						var result = await connection.ExecuteAsync(query, new { DateToday = dateToday, ID = Id }, transaction: transaction);

						transaction.Commit();
						return result;
					}
					catch
					{
						transaction.Rollback();
						throw;
					}
				}
			}
		}

		public async Task<int> InsertUserAsync(UserAccount userAccount)
		{
			var parameters = new DynamicParameters();
			parameters.Add("@Id", Guid.NewGuid());
			parameters.Add("@Username", userAccount.UserName);
			parameters.Add("@Password", userAccount.Password);
			parameters.Add("@Email", userAccount.Email);
			parameters.Add("@Name", userAccount.Name);
			parameters.Add("@Phone", userAccount.Phone);

			var query = @"INSERT INTO Users 
                  (Id, Username, Password, Email, Name, Phone) 
                  VALUES 
                  (@Id, @Username, @Password, @Email, @Name, @Phone)";

			using (var connection = _connectionFactory())
			{
				connection.Open();

				using (var transaction = connection.BeginTransaction())
				{
					try
					{
						var result = await connection.ExecuteAsync(query, parameters, transaction: transaction);

						transaction.Commit();
						return result;
					}
					catch
					{
						transaction.Rollback();
						throw;
					}
				}
			}
		}

        public async Task<int> UpdateUserAsync(UserAccount userAccount)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", userAccount.Id);
            parameters.Add("@Username", userAccount.UserName);			
            parameters.Add("@Email", userAccount.Email);
            parameters.Add("@Name", userAccount.Name);
            parameters.Add("@Phone", userAccount.Phone);
			string query = String.Empty;
            if (!string.IsNullOrEmpty(userAccount.Password))
            {
                parameters.Add("@Password", userAccount.Password);
                query = @"Update Users SET Username = @Username, Password = @Password ,Email = @Email, Name = @Name, Phone = @Phone Where Id=@Id";
            }
			else
			{
                query = @"Update Users SET Username = @Username, Email = @Email, Name = @Name, Phone = @Phone Where Id=@Id";
            }
          

            using (var connection = _connectionFactory())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await connection.ExecuteAsync(query, parameters, transaction: transaction);

                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        public async Task<int> UpdateRoleAsync(string roleId, string userId)
		{
			var dateToday = _dateTimeHelper.GetCurrentUtc();
			var query = "usp_UpdateInsert_UserRole";

			using (var connection = _connectionFactory())
			{
				connection.Open();

				using (var transaction = connection.BeginTransaction())
				{
					try
					{
						var result = await connection.ExecuteAsync(query, new { RoleId = roleId, UserId = userId }, transaction: transaction, commandType: CommandType.StoredProcedure);

						transaction.Commit();
						return result;
					}
					catch
					{
						transaction.Rollback();
						throw;
					}
				}
			}
		}

		public async Task<bool> UserNameExistsAsync(string userName)
		{
			var query = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName";

			using (var connection = _connectionFactory())
			{
				connection.Open();
				var result = await connection.ExecuteScalarAsync<int>(query, new { UserName = userName });
				return result == 0;
			}
		}

		public async Task<bool> UserNameExistsAsync(string userName, string userId)
		{
			var query = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName AND Id <> @UserId";

			using (var connection = _connectionFactory())
			{
				connection.Open();
				var result = await connection.ExecuteScalarAsync<int>(query, new { UserName = userName, UserId = userId });
				return result == 1 || result == 0;
			}
		}

		public async Task<bool> EmailExistsAsync(string email)
		{
			var query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";

			using (var connection = _connectionFactory())
			{
				connection.Open();
				var result = await connection.ExecuteScalarAsync<int>(query, new { Email = email });
				return result == 0;
			}
		}

		public async Task<bool> EmailExistsAsync(string email, string userId)
		{
			var query = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND Id <> @UserId";

			using (var connection = _connectionFactory())
			{
				connection.Open();
				var result = await connection.ExecuteScalarAsync<int>(query, new { Email = email, UserId = userId });
				return result == 1 || result == 0;
			}
		}

		public async Task<bool> PhoneExistsAsync(string phone)
		{
			var query = "SELECT COUNT(1) FROM Users WHERE Phone = @Phone";

			using (var connection = _connectionFactory())
			{
				connection.Open();
				var result = await connection.ExecuteScalarAsync<int>(query, new { Phone = phone });
				return result == 0;
			}
		}

		public async Task<bool> PhoneExistsAsync(string phone, string userId)
		{
			var query = "SELECT COUNT(1) FROM Users WHERE Phone = @Phone AND Id <> @UserId";

			using (var connection = _connectionFactory())
			{
				connection.Open();
				var result = await connection.ExecuteScalarAsync<int>(query, new { Phone = phone, UserId = userId });
				return result == 1 || result == 0;
			}
		}
	}
}
