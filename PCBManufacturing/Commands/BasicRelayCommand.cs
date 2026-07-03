using System.Windows.Input;

namespace PCBManufacturing.Commands
{
	public class BasicRelayCommand : ICommand
	{
		private readonly Action m_execute;
		private readonly Func<bool>? m_canExecute;

		public bool CanExecute(object? parameter)
		{
			return m_canExecute == null || m_canExecute();
		}

		public void Execute(object? parameter)
		{
			m_execute();
		}

		public BasicRelayCommand(Action execute, Func<bool>? canExecute = null)
		{
			m_execute = execute ?? throw new ArgumentNullException(nameof(execute));
			m_canExecute = canExecute;
		}

		public event EventHandler? CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}
