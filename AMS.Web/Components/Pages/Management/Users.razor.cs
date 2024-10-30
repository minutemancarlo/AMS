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
                Position = DialogPosition.TopCenter,
                BackgroundClass = "dialogBlur",
                FullWidth = true,
                CloseButton = true,
                MaxWidth=MaxWidth.Small
            };

            var parameters = new DialogParameters<UserDialog>()
            {
                {x=>x.Action, StringConstants.View},
				{x=>x.userAccount, dto}
			};
            await DialogService.ShowAsync<UserDialog>("Simple Dialog", parameters,options);
        }

		private async Task OnUpdateUserInfo(UserAccount dto)
		{
			var options = new DialogOptions
			{
				CloseOnEscapeKey = true,
				BackdropClick = false,
				Position = DialogPosition.TopCenter,
				BackgroundClass = "dialogBlur",
				FullWidth = true,
				CloseButton = true,
				MaxWidth = MaxWidth.Small
			};

			var parameters = new DialogParameters<UserDialog>()
			{
				{x=>x.Action, StringConstants.Update},
				{x=>x.userAccount, dto}
			};
			await DialogService.ShowAsync<UserDialog>("Simple Dialog", parameters, options);
		}

		private async Task OnAddUserInfo()
        {
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                BackdropClick = false,
                Position = DialogPosition.TopCenter,
                BackgroundClass = "dialogBlur",
                FullWidth = true,
                CloseButton = true,
                MaxWidth = MaxWidth.Small
            };

            var parameters = new DialogParameters<UserDialog>()
            {
                {x=>x.Action, StringConstants.Add},
                {x=>x.userAccount, new UserAccount()}
            };
            await DialogService.ShowAsync<UserDialog>("Simple Dialog", parameters, options);
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
                Snackbar.Add(ex.Message,Severity.Error);
            }
            finally
            {
				_loading = false;
			}
		}

    }
}
