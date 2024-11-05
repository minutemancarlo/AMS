using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Data.Models.Entities
{
	public class EnergyInput
	{
		public int Id { get; set; }
		public DateTime? ReadingDate { get; set; }
		public ReadingInput ReadingInput { get; set; } = new();
		public int? UnitNumber { get; set; }		
		public decimal TotalKwhUsed { get; set; }
		public decimal TotalComsumption { get; set; }
		public bool IsMainInput { get; set; } = true;

	}


	public partial class ReadingInput : BaseModel
	{
		public decimal PreviousReadingKwh { get; set; } = 0.00M;
		public decimal PresentReadingKwh { get; set; } = 0.00M;
		public decimal PreviousConsumption { get; set; } = 0.00M;
		public decimal PresentConsumption { get; set; } = 0.00M;
	}


}
