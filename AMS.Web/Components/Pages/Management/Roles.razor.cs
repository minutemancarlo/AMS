using AMS.Data.Models.Entities;
using AMS.Web.Authentication;
using Microsoft.AspNetCore.Components;

namespace AMS.Web.Components.Pages.Management
{
	public partial class Roles: ComponentBase
	{
		#region Dependecy Injections
		[Inject] UserAccountService userAccountService { get; set; }
		#endregion
		#region Instances

		#endregion
		#region Properties
		private List<UserRoles> userRoles = new();
		private bool _loading = false;
		private string? _searchString;
		#endregion


		protected override async Task OnInitializedAsync()
		{
			userRoles = await userAccountService.GetAllUserRoles();
			await base.OnInitializedAsync();
		}



	}
}
