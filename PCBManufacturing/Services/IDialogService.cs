namespace PCBManufacturing.Services
{
	public interface IDialogService
	{
		void ShowMessage(string message);

		bool Confirm(string message);
	}
}
