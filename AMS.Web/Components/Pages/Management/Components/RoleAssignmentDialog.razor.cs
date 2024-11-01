using AMS.Data.Models.Entities;
using AMS.Web.Components.Pages.General_Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;
using MudExtensions;

namespace AMS.Web.Components.Pages.Management.Components
{
    public partial class RoleAssignmentDialog : ComponentBase
    {
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }

        [Inject] ProtectedSessionStorage _sessionStorage { get; set; }
        [Inject] IDialogService DialogService { get; set; }
        [Inject] ISnackbar Snackbar { get; set; }

        MudTransferList<UserRoles> _transferList = new();

        [Parameter]
        public ICollection<UserRoles> unAssignedRoles { get; set; }
        [Parameter]
        public ICollection<UserRoles> assignedRoles { get; set; }

        private bool isDarkMode = false;     

		protected override async Task OnInitializedAsync()
		{
            var result = await _sessionStorage.GetAsync<bool>("DarkModeEnabled");
            isDarkMode = result.Value;
			await base.OnInitializedAsync();
		}

        private async void Submit()
        {
            var valuesEnd = _transferList.GetEndListSelectedValues();

            if (valuesEnd.FirstOrDefault()?.RoleId == null)
            {
                Snackbar.Add("Please select 1 role",Severity.Error);
                return;
            }            

            if (!await ConfirmDialog(valuesEnd.FirstOrDefault()?.RoleName))
            {
                MudDialog.Close(DialogResult.Ok(valuesEnd.ToList()));
            }

            
        }

        private async Task<bool> ConfirmDialog(string roleName)
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
                    { x => x.Message, $"Are you sure you want to set role {roleName} to this account?"}
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
