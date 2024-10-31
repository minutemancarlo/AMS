using AMS.Data.Models;
using AMS.Data.Models.Entities;
using AMS.Data.Models.Validations;
using AMS.Data.Repositories.Authentication;
using AMS.Web.Authentication;
using Blazor.SubtleCrypto;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace AMS.Web.Components.Pages.Management.Components
{
	public partial class UserDialog : ComponentBase
	{
		#region Dependency Injections
		[Inject] ISnackbar Snackbar { get; set; }
		[Inject] IUserManagementRepository UserManagementRepository { get; set; }
		[Inject] UserAccountService UserAccountService { get; set; }
		[Inject] ICryptoService Crypto { get; set; }
		#endregion

		#region Parameters
		[CascadingParameter]
		private MudDialogInstance MudDialog { get; set; }

		[Parameter] public string Action { get; set; }
		[Parameter] public UserAccount userAccount { get; set; }
		#endregion

		#region Properties
		private bool showPassword = false;
		private bool showConfirmPassword = false;
		private bool isReadOnly = false;
		private bool _loading = false;
		private AddUserValidator addUserValidator;
		private UpdateUserValidator updateUserValidator;
		MudForm? userForm;
		
		#endregion

		protected override Task OnParametersSetAsync()
		{
			userAccount.Password = String.Empty;
			userAccount.ConfirmPassword = String.Empty;
			userAccount.isActive = Action == StringConstants.Add ? true : Action == StringConstants.Update || Action == StringConstants.View? userAccount.isActive: userAccount.isActive;
			isReadOnly = Action == StringConstants.View ? true : false;
			addUserValidator = new AddUserValidator(UserManagementRepository);
			updateUserValidator = new UpdateUserValidator(UserManagementRepository);
			return base.OnParametersSetAsync();
		}

      


        private async Task Submit()
		{
			try
			{
				_loading = true;

				await userForm.Validate();

				if (!userForm.IsValid)
				{
					return;
				}

				if (Action == StringConstants.Add)
				{
					CryptoResult encrypted = await Crypto.EncryptAsync(userAccount.Password);
					userAccount.Password = encrypted.Value;
					var result = await UserAccountService.InsertUser(userAccount);
					Snackbar.Add($"Submitted: {result}", Severity.Success);
				}



				//MudDialog.Close(DialogResult.Ok(true));
			}
			catch (Exception ex)
			{
				Snackbar.Add($"{ex.Message}", Severity.Error);
			}
			finally
			{
				_loading = false;
			}

		}

		private void Cancel() => MudDialog.Cancel();
	}
}
