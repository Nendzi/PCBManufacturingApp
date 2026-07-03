using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace PCBManufacturing.Controls
{
	public partial class DropdownListWithLabel : UserControl
	{
		public DropdownListWithLabel()
		{
			InitializeComponent();
		}

		// ShowColorSquares
		public static readonly DependencyProperty ShowColorSquaresProperty =
				DependencyProperty.Register(
						nameof(ShowColorSquares),
						typeof(bool),
						typeof(DropdownListWithLabel),
						new PropertyMetadata(false));

		public bool ShowColorSquares
		{
			get => (bool)GetValue(ShowColorSquaresProperty);
			set => SetValue(ShowColorSquaresProperty, value);
		}

		// LabelText
		public static readonly DependencyProperty LabelTextProperty =
				DependencyProperty.Register(
						nameof(LabelText),
						typeof(string),
						typeof(DropdownListWithLabel),
						new PropertyMetadata("Label:"));

		public string LabelText
		{
			get => (string)GetValue(LabelTextProperty);
			set => SetValue(LabelTextProperty, value);
		}

		// Items
		public static readonly DependencyProperty ItemsProperty =
				DependencyProperty.Register(
						nameof(Items),
						typeof(IEnumerable),
						typeof(DropdownListWithLabel),
						new PropertyMetadata(null));

		public IEnumerable Items
		{
			get => (IEnumerable)GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}

		// SelectedItem
		public static readonly DependencyProperty SelectedItemProperty =
				DependencyProperty.Register(
						nameof(SelectedItem),
						typeof(object),
						typeof(DropdownListWithLabel),
						new FrameworkPropertyMetadata(
								null,
								FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public object SelectedItem
		{
			get => GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}
	}
}