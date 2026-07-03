using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PCBManufacturing.Converters
{
	public class SelectedWidthToFontWeightConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			bool twoValues = values.Length == 2;
			bool bothAreDouble = values[0] is double && values[1] is double;
			bool bothAreInt = values[0] is int && values[1] is int;

			if (twoValues)
			{
				if (bothAreDouble)
				{
					double itemWidth = (double)values[0];
					double selectedWidth = (double)values[1];
					if (itemWidth == selectedWidth)
					{
						return FontWeights.Bold;
					}
				}
				else if (bothAreInt)
				{
					int itemWidth = (int)values[0];
					int selectedWidth = (int)values[1];
					if (itemWidth == selectedWidth)
					{
						return FontWeights.Bold;
					}
				}
				else
				{
					return FontWeights.Regular;
				}
			}
			return FontWeights.Regular;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
