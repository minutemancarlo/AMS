using AMS.Data.Models;
using AMS.Data.Models.Entities;
using AMS.Data.Models.Validations;
using AMS.Data.Repositories.Authentication;
using AMS.Web.Authentication;
using AMS.Web.Components.Pages.General_Components;
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
		[Inject] IDialogService DialogService { get; set; }
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

               if(await ConfirmDialog())
				{
					return;
				}

                if (Action == StringConstants.Add)
				{
					CryptoResult encrypted = await Crypto.EncryptAsync(userAccount.Password);
					userAccount.Password = encrypted.Value;
					await UserAccountService.InsertUser(userAccount);
					Snackbar.Add($"User Account Added", Severity.Success);
				}
				else
				{
					if (!string.IsNullOrEmpty(userAccount.Password))
					{
                        CryptoResult encrypted = await Crypto.EncryptAsync(userAccount.Password);
                        userAccount.Password = encrypted.Value;
                    }
                    await UserAccountService.UpdateUser(userAccount);
                    Snackbar.Add($"User Account Updated", Severity.Success);

                }
				MudDialog.Close(DialogResult.Ok(true));
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

		private async Task<bool> ConfirmDialog()
		{
            DialogOptions options = new DialogOptions()
            {
                CloseOnEscapeKey = true,
                BackdropClick = false,
                Position = DialogPosition.Center,
                BackgroundClass = "dialogBlur",
                FullWidth = true,
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall
            };
            var parameters = new DialogParameters<ConfirmDialog>
                {
                    { x => x.Title, "Confirm"},
                    { x => x.Icon, "fa-circle-question"},
                    { x => x.Color, Color.Info},
                    { x => x.Message, $"Are you sure you want to {Action.ToLower()} this account?"}
                };
            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Simple Dialog", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
               return true;
            }
            return false;

        }

        private void Cancel() => MudDialog.Cancel();
	}
}
