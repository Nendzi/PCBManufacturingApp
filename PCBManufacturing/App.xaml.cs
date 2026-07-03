using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using PCBManufacturing.Services;
using PCBManufacturing.ViewModels;

namespace PCBManufacturing
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public IServiceProvider? Services { get; }

		public App()
		{
			var services = new ServiceCollection();

			ConfigureServices(services);

			Services = services.BuildServiceProvider();
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IDialogService, MessageBoxService>();
			services.AddTransient<MainWindowViewModel>();
			services.AddSingleton<MainWindow>();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			if (Services != null)
			{
				var mainWindow = Services.GetRequiredService<MainWindow>();
				mainWindow.Show();
			}

			base.OnStartup(e);
		}
	}

}
