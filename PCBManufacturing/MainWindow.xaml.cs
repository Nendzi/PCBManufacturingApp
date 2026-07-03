using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

using PCBManufacturing.Services;
using PCBManufacturing.ViewModels;

namespace PCBManufacturing
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow(MainWindowViewModel mv)
		{
			InitializeComponent();
			DataContext = mv;
		}

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			var animation = new DoubleAnimation
			{
				From = 0,
				To = 360,
				Duration = TimeSpan.FromSeconds(10),
				RepeatBehavior = RepeatBehavior.Forever,
				IsCumulative = true
			};

			PCBRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
		}

		private void WidthTextBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (sender is TextBox textBox && textBox.DataContext is double selectedWidth)
			{
				if (DataContext is MainWindowViewModel viewModel)
				{
					viewModel.Width = selectedWidth;
				}
			}
		}

		private void LenghtTextBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (sender is TextBox textBox && textBox.DataContext is double selectedWidth)
			{
				if (DataContext is MainWindowViewModel viewModel)
				{
					viewModel.Length = selectedWidth;
				}
			}
		}

		private void LeyersQtyTextBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (sender is TextBox textBox && textBox.DataContext is int selectedWidth)
			{
				if (DataContext is MainWindowViewModel viewModel)
				{
					viewModel.LayerQty = selectedWidth;
				}
			}
		}
	}
}