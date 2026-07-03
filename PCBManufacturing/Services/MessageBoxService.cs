using System.Windows;

namespace PCBManufacturing.Services
{
	public class MessageBoxService : IDialogService
	{
		public bool Confirm(string message) => MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

		public void ShowMessage(string message)
		{
			MessageBox.Show(message, "Information", MessageBoxButton.OK);
		}
	}
}
