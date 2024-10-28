using AMS.Data.Models.Entities;
using AMS.Data.Models.Validations;
using AMS.Web.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Reflection;

namespace AMS.Web.Components.Pages.Auth
{
    public partial class Login : ComponentBase
    {
        #region Dependency Injections
        [Inject] ISnackbar Snackbar { get; set; }
        [Inject] UserAccountService userAccountService { get; set; }
        [Inject] AuthenticationStateProvider authStateProvider { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        #endregion

        #region Properties
        private string? version;
        private bool showPassword = false;
        #endregion

        #region Instances
        private UserAccount loginModel = new();
        LoginValidator loginValidator = new();
        MudForm? loginForm;        
        #endregion

        protected override async Task OnInitializedAsync()
        {
            version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                Navigation.NavigateTo("/");
            }
            await base.OnInitializedAsync();
        }

        private async Task Authenticate()
        {
            if (!await ValidateModel())
            {
                return;
            }

            var userAccount = userAccountService.GetByUserName(loginModel.UserName);
            if (userAccount == null || userAccount.Password != loginModel.Password)
            {
                Snackbar.Add("Invalid Credentials", Severity.Error);
                return;
            }

            var customAuthStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
            await customAuthStateProvider.UpdateAuthenticationState(new UserSession
            {
                Id = userAccount.Id,
                Email = userAccount.Email,
                Name = userAccount.Name,
                UserName = userAccount.UserName,
                Role = userAccount.Role
            });
            Navigation.NavigateTo("/", true);
        }

        private async Task<bool> ValidateModel()
        {
            await loginForm.Validate();
            return loginForm.IsValid;
        }
    }
}
