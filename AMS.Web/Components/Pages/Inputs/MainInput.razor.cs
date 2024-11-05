using AMS.Data.Models.Entities;
using Microsoft.AspNetCore.Components;
using AMS.Web.Components.Pages.Inputs.Components;
using MudBlazor;
using AMS.Data.Models;
using AMS.Web.Components.Pages.Management.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using AMS.Web.Components.Pages.Management;
namespace AMS.Web.Components.Pages.Inputs
{
	public partial class MainInput : ComponentBase
	{
		[Inject]
        IDialogService DialogService { get; set; }
        [Inject] ProtectedSessionStorage _sessionStorage { get; set; }
        private List<EnergyInput> mainEnergyInput = new();
		private bool _loading = false;

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

        protected override async Task OnInitializedAsync()
		{
			var energyInput1 = new EnergyInput
			{
				Id = 1,
				ReadingDate = DateTime.Now,
				ReadingInput = new ReadingInput
				{
					PreviousReadingKwh = 500.00M,
					PresentReadingKwh = 550.00M,
					PreviousConsumption = 200.00M,
					PresentConsumption = 50.00M,
					CreatedBy = "User1",
					CreatedOn = DateTime.Now.AddMonths(-1),
					UpdatedBy = "User1",
					UpdatedOn = DateTime.Now
				},
				UnitNumber = 101,
				TotalKwhUsed = 50.00M,
				TotalComsumption = 250.0000M
			};
			mainEnergyInput.Add(energyInput1);

			var energyInput2 = new EnergyInput
			{
				Id = 2,
				ReadingDate = DateTime.Now,
				ReadingInput = new ReadingInput
				{
					PreviousReadingKwh = 1000.00M,
					PresentReadingKwh = 1100.00M,
					PreviousConsumption = 400.00M,
					PresentConsumption = 100.00M,
					CreatedBy = "User2",
					CreatedOn = DateTime.Now.AddMonths(-2),
					UpdatedBy = "User2",
					UpdatedOn = DateTime.Now
				},
				UnitNumber = 102,
				TotalKwhUsed = 100.00M,
				TotalComsumption = 500.00M
			};

			mainEnergyInput.Add(energyInput2);

			await base.OnInitializedAsync();
		}


		private async Task OnShowInputDialog(EnergyInput input, string action)
		{
			var session = await _sessionStorage.GetAsync<UserSession>("UserSession");
            var userId = session.Value.Id;
            var parameters = new DialogParameters<EnergyInputDialog>()
            {
                {x=>x.Action, action},
                {x=>x.userId, userId},
                {x=>x.input, input}

            };
            var dialog = await DialogService.ShowAsync<EnergyInputDialog>("Simple Dialog", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                
            }
        }


	}
}
