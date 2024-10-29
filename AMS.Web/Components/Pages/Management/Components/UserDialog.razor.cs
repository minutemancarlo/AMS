using AMS.Data.Models.Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace AMS.Web.Components.Pages.Management.Components
{
	public partial class UserDialog : ComponentBase
	{
		#region Dependency Injections

		#endregion

		#region Parameters
		[CascadingParameter]
		private MudDialogInstance MudDialog { get; set; }
		[Parameter] public string Action { get; set; }
		[Parameter] public UserAccount userAccount { get; set; }
		#endregion

		#region Properties

		#endregion

		private void Submit() => MudDialog.Close(DialogResult.Ok(true));

		private void Cancel() => MudDialog.Cancel();
	}
}
