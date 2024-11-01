using AMS.Data.Models.Entities;
using AMS.Web.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using AMS.Web.Components.Pages.Management.Components;
using AMS.Data.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Identity.Client;
using AMS.Web.Components.Pages.General_Components;

namespace AMS.Web.Components.Pages.Management
{
	public partial class Users : ComponentBase
	{
		#region Dependency Injections
		[Inject] ISnackbar Snackbar { get; set; }
		[Inject] IDialogService DialogService { get; set; }
		[Inject] UserAccountService userAccountService { get; set; }
		[Inject] ProtectedSessionStorage _sessionStorage { get; set; }
		#endregion

		#region Properties
		private List<UserAccount> _userAccount = new();
		private string? _searchString;
		private bool _loading = false;
		private ICollection<UserRoles> userRoles = new List<UserRoles>();
		DialogOptions options = new DialogOptions()
		{
			CloseOnEscapeKey = true,
			BackdropClick = false,
			Position = DialogPosition.TopCenter,
			BackgroundClass = "dialogBlur",
			FullWidth = true,
			CloseButton = true,
			MaxWidth = MaxWidth.Small
		};
		private string? userId;
		#endregion

		protected override async Task OnInitializedAsync()
		{
			await LoadUserData();
			var userResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
			userId = userResult.Value.Id;
			await base.OnInitializedAsync();
		}

		private async Task OnViewUserInfo(UserAccount dto)
		{
			var parameters = new DialogParameters<UserDialog>()
			{
				{x=>x.Action, StringConstants.View},
				{x=>x.userAccount, dto}
			};
			await DialogService.ShowAsync<UserDialog>("Simple Dialog", parameters, options);
		}

		private async Task OnUpdateUserInfo(UserAccount dto)
		{

			var parameters = new DialogParameters<UserDialog>()
			{
				{x=>x.Action, StringConstants.Update},
				{x=>x.userAccount, dto}
			};
			 var dialog = await DialogService.ShowAsync<UserDialog>("Simple Dialog", parameters, options);
			var result = await dialog.Result;
			if (!result.Canceled)
			{
				await LoadUserData();
			}
		}

		private async Task OnAddUserInfo()
		{


			var parameters = new DialogParameters<UserDialog>()
			{
				{x=>x.Action, StringConstants.Add},
				{x=>x.userAccount, new UserAccount()}
			};
			var dialog = await DialogService.ShowAsync<UserDialog>("Simple Dialog", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadUserData();
            }
        }

		private async Task OnUpdateRole(UserAccount user)
		{
			try
			{
				userRoles = await userAccountService.GetAllUserRoles();
				var itemsToRemove = userRoles.Where(x => x.RoleName == user.Role).ToList();
				foreach (var item in itemsToRemove)
				{
					userRoles.Remove(item);
				}
				var parameters = new DialogParameters<RoleAssignmentDialog>
				{
					{ x => x.unAssignedRoles, userRoles },
					{ x => x.assignedRoles, itemsToRemove }
				};
				var dialog = await DialogService.ShowAsync<RoleAssignmentDialog>("Simple Dialog", parameters, options);
				var result = await dialog.Result;
				if (!result.Canceled)
				{
					var rolesList = result.Data as List<UserRoles>;
					if (user.Role != rolesList.FirstOrDefault()?.RoleName)
					{
						await userAccountService.UpdateRole(rolesList.FirstOrDefault()?.RoleId, user.Id);
						await LoadUserData();
						Snackbar.Add(new MarkupString($"User <strong>{user.UserName}</strong> role updated to <strong>{rolesList.FirstOrDefault()?.RoleName}</strong>"), Severity.Success);
					}
				}
			}
			catch (Exception ex)
			{
				Snackbar.Add($"{ex.Message}", Severity.Error);
			}			
		}

		private async Task OnChangeStatus(UserAccount user)
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
					{ x => x.Message, $"Are you sure you want to {(user.isActive?"disable":"enable")} this account?"}
				};
            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Simple Dialog", parameters, options);
        }

		private async Task LoadUserData()
		{
			try
			{
				_loading = true;
				_userAccount = await userAccountService.GetAllUserAccounts();
			}
			catch (Exception ex)
			{
				Snackbar.Add(ex.Message, Severity.Error);
			}
			finally
			{
				_loading = false;
			}
		}

	}
}
