using AMS.Data.Models.Entities;
using AMS.Web.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using AMS.Web.Components.Pages.Management.Components;
using AMS.Data.Models;

namespace AMS.Web.Components.Pages.Management
{
    public partial class Users : ComponentBase
    {
        #region Dependency Injections
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] IDialogService DialogService { get; set; }
		[Inject] UserAccountService userAccountService { get; set; }
		#endregion

		#region Properties
		private List<UserAccount> _userAccount = new();
        private string? _searchString;
        private bool _loading = false;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await LoadUserData();
			await base.OnInitializedAsync();
        }

        private async Task OnViewUserInfo(UserAccount dto)
        {
            var options = new DialogOptions 
            { 
                CloseOnEscapeKey = true, 
                BackdropClick = false, 
                Position = DialogPosition.Center,
                BackgroundClass = "dialogBlur",
                FullWidth = true,
                MaxWidth=MaxWidth.Small
            };

            var parameters = new DialogParameters<UserDialog>()
            {
                {x=>x.Action, StringConstants.View},
				{x=>x.userAccount, dto}
			};
            await DialogService.ShowAsync<UserDialog>("Simple Dialog", parameters,options);
        }

        private async Task LoadUserData()
        {
            try
            {
                _loading = true;
                await Task.Delay(2000);
                _userAccount = await userAccountService.GetAllUserAccounts();
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message,Severity.Error);
            }
            finally
            {
				_loading = false;
			}
		}

    }
}
