using AMS.Data.Models.Entities;
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

        private void Submit()
        {
            var valuesEnd = _transferList.GetEndListSelectedValues();
			MudDialog.Close(DialogResult.Ok(valuesEnd.ToList()));
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
