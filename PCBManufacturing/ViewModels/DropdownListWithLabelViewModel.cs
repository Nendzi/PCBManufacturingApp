using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using PCBManufacturing.Models;

namespace PCBManufacturing.ViewModels
{
	public class DropdownListWithLabelViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private bool m_showColorSquares = true;
		private ObservableCollection<OptionItemModel> m_options = new();
		private OptionItemModel? m_selectedOption;

		public string? Label { get; set; } = "a";

		public bool ShowColorSquares
		{
			get => m_showColorSquares;
			set
			{
				m_showColorSquares = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<OptionItemModel> Options
		{
			get => m_options;
			set
			{
				m_options = value;
				OnPropertyChanged();

				if (m_options != null && m_options.Count > 0)
				{
					SelectedOption = Options[0];
				}
			}
		}

		public OptionItemModel? SelectedOption
		{
			get => m_selectedOption;
			set { m_selectedOption = value; OnPropertyChanged();}
		}

		public DropdownListWithLabelViewModel(bool showColor)
		{
			m_showColorSquares = showColor;
		}
		
		protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
