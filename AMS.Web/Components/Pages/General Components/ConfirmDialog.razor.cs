using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AMS.Web.Components.Pages.General_Components
{
    public partial class ConfirmDialog: ComponentBase
    {
        #region Dependency Injections

        #endregion

        #region Parameters
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Icon { get; set; }
        [Parameter] public Color Color { get;set; }
        [Parameter] public string Message { get; set; }
        #endregion

        private void Submit() => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() => MudDialog.Cancel();
    }
}
