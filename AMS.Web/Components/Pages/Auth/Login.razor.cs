using AMS.Data.Models.Entities;
using AMS.Data.Models.Validations;
using AMS.Web.Authentication;
using Blazor.SubtleCrypto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
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
        [Inject] ICryptoService Crypto { get; set; }
        [Inject] ProtectedSessionStorage _sessionStorage { get; set; }
        #endregion

        #region Properties
        private string? version;
        private bool showPassword = false;
        
        #endregion

        #region Instances
        private UserAccount loginModel = new();
        private RememberMe RememberMe = new();
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var rememberMe = await _sessionStorage.GetAsync<RememberMe>("IsPersisted");
            var rememberValue = rememberMe.Success ? rememberMe.Value : null;
            if (rememberValue != null)
            {
                if (rememberValue.Remember)
                {
                    RememberMe.Remember = rememberValue.Remember;
                    RememberMe.UserName = rememberValue.UserName;
                    RememberMe.Password = rememberValue.Password;
                    loginModel.UserName = RememberMe.UserName;
                    loginModel.Password = RememberMe.Password;
                    StateHasChanged();
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task Authenticate()
        {
            if (!await ValidateModel())
            {
                return;
            }

            var userAccount = await userAccountService.GetByUserName(loginModel.UserName);
            if (userAccount == null)
            {
                Snackbar.Add("Invalid Credentials", Severity.Error);
                return;
            }

            if (RememberMe.Remember)
            {
                RememberMe.Remember = RememberMe.Remember;
                RememberMe.UserName = loginModel.UserName;
                RememberMe.Password = loginModel.Password;
                await _sessionStorage.SetAsync("IsPersisted", RememberMe);
            }
            else
            {
                await _sessionStorage.DeleteAsync("IsPersisted");
            }

            string decryptedPassword = await Crypto.DecryptAsync(userAccount.Password);
            if (decryptedPassword != loginModel.Password)
            {
                Snackbar.Add("Password does not match.", Severity.Error);
                return;
            }

            if (!userAccount.isActive)
            {
                Snackbar.Add("Your account has been disabled. Please contact your administrator.", Severity.Error);
                return;
            }

            await userAccountService.UpdateLoginDates(userAccount.Id);

            var customAuthStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
            await customAuthStateProvider.UpdateAuthenticationState(new UserSession
            {
                Id = userAccount.Id,
                Email = userAccount.Email,
                Name = userAccount.Name,
                UserName = userAccount.UserName,
                Role = userAccount.Role,
                LastLoginDate = userAccount.LastLoginDate
            });

            
            Navigation.NavigateTo("/", true);
        }

        private async Task KeyDownSubmit(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await Authenticate();
            }
        }

        private async Task<bool> ValidateModel()
        {
            await loginForm.Validate();
            return loginForm.IsValid;
        }
    }
}
