using System.Globalization;
using System.Windows.Controls;

namespace PCBManufacturing.Validators
{
	public class PostalCodeValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value == null || string.IsNullOrWhiteSpace(value?.ToString()))
			{
				return new ValidationResult(false, "Postal code is required.");
			}

			string postalCode = value.ToString()!.Trim();

			if (!int.TryParse(postalCode, out int numericValue))
			{
				return new ValidationResult(false, "Postal code must contain only digits.");
			}

			if (numericValue < 10000 || numericValue > 99999)
			{
				return new ValidationResult(false, @"Postal code must be exactly 5 digits and cannot start with 0.");
			}

			return new ValidationResult(true, null);
		}
	}
}
