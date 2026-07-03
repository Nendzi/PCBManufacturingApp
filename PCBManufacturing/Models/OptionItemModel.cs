using System.Windows.Media;

namespace PCBManufacturing.Models
{
	public class OptionItemModel
	{
		public long Id { get; set; }
		public string Category { get; set; } = "";
		public string DisplayName { get; set; } = "";
		public object? Value { get; set; }
		public Brush? ColorValue { get; set; }

		public long AvaiableMaterialId { get; set; }

		public long AvaiableMaskId { get; set; }

		public int DaysExtend { get; set; }

		public decimal PriceRaise { get; set; }
		public override string ToString() => DisplayName;
	}
}
