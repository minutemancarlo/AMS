using AMS.Data.Models.Entities;
using AMS.Data.Repositories.Authentication;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AMS.Data.Models.Validations
{

	public class AddUserValidator : AbstractValidator<UserAccount>
	{
		private readonly IUserManagementRepository _userManagementRepository;
		public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
		{
			var result = await ValidateAsync(ValidationContext<UserAccount>.CreateWithOptions((UserAccount)model, x => x.IncludeProperties(propertyName)));
			if (result.IsValid)
				return Array.Empty<string>();
			return result.Errors.Select(e => e.ErrorMessage);
		};
		public AddUserValidator(IUserManagementRepository userManagementRepository)
		{
			_userManagementRepository = userManagementRepository;

			RuleFor(x => x.UserName)
				.NotEmpty().WithMessage("Username is required.")
				.Length(1, 100)
				.MustAsync(async (username, cancellation) => await UserNameExists(username))
				.WithMessage("Username already exists.");

			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.")
				.MustAsync(async (email, cancellation) => await EmailExists(email))
				.WithMessage("Email already exists.");

			RuleFor(x => x.Phone)
				.NotEmpty().WithMessage("Phone number is required.")
				.Matches(new Regex(@"^09\d{9}$")).WithMessage("Phone number must be 11 digits and start with '09'.")
				.MustAsync(async (phone, cancellation) => await PhoneExists(phone))
				.WithMessage("Phone number already exists.");

			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Name is required.")
				.Length(1, 100);

			RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password is required.")
			.Length(8, 100).WithMessage("Password must be between 8 and 100 characters.")
			.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
			.Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
			.Matches(@"\d").WithMessage("Password must contain at least one number.")
			.Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

			RuleFor(x => x.ConfirmPassword)
				.Equal(x => x.Password).WithMessage("Confirm password must match the password.");
		}

		private async Task<bool> UserNameExists(string userName) => await _userManagementRepository.UserNameExistsAsync(userName);

		private async Task<bool> EmailExists(string email) => await _userManagementRepository.EmailExistsAsync(email);

		private async Task<bool> PhoneExists(string phone) => await _userManagementRepository.PhoneExistsAsync(phone);
	}

	public class UpdateUserValidator : AbstractValidator<UserAccount>
	{
		private readonly IUserManagementRepository _userManagementRepository;
		public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
		{
			var result = await ValidateAsync(ValidationContext<UserAccount>.CreateWithOptions((UserAccount)model, x => x.IncludeProperties(propertyName)));
			if (result.IsValid)
				return Array.Empty<string>();
			return result.Errors.Select(e => e.ErrorMessage);
		};
		public UpdateUserValidator(IUserManagementRepository userManagementRepository)
		{
			_userManagementRepository = userManagementRepository;

			RuleFor(x => x.UserName)
				.NotEmpty().WithMessage("Username is required.")
				.Length(1, 100)
				.MustAsync(async (x, username, cancellation) => await UserNameExists(username, x.Id))
				.WithMessage("Username already exists.");

			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.")
				.MustAsync(async (x, email, cancellation) => await EmailExists(email, x.Id))
				.WithMessage("Email already exists.");

			RuleFor(x => x.Phone)
				.NotEmpty().WithMessage("Phone number is required.")
				.Matches(new Regex(@"^09\d{9}$")).WithMessage("Phone number must be 11 digits and start with '09'.")
				.MustAsync(async (x, phone, cancellation) => await PhoneExists(phone, x.Id))
				.WithMessage("Phone number already exists.");

			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Name is required.")
				.Length(1, 100);

			RuleFor(x => x.Password)
				.NotEmpty().When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password is required.")
				.Length(8, 100).When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password must be between 8 and 100 characters.")
				.Matches(@"[A-Z]").When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password must contain at least one uppercase letter.")
				.Matches(@"[a-z]").When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password must contain at least one lowercase letter.")
				.Matches(@"\d").When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password must contain at least one number.")
				.Matches(@"[\W_]").When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Password must contain at least one special character.");

			RuleFor(x => x.ConfirmPassword)
				.Equal(x => x.Password).When(x => !string.IsNullOrEmpty(x.Password)).WithMessage("Confirm password must match the password.");

		}

		private async Task<bool> UserNameExists(string userName, string Id) => await _userManagementRepository.UserNameExistsAsync(userName, Id);

		private async Task<bool> EmailExists(string email, string Id) => await _userManagementRepository.EmailExistsAsync(email, Id);

		private async Task<bool> PhoneExists(string phone, string Id) => await _userManagementRepository.PhoneExistsAsync(phone, Id);
	}

}
