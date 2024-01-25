using System.Windows;
using Microsoft.Extensions.Hosting;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static IHost AppHost { get; private set; } = null!;

		private static void Main()
		{
			var hostBuilder = Host.CreateApplicationBuilder();
			hostBuilder.Services.AddWpfApplication<App>();
			AppHost = hostBuilder.Build();
			AppHost.Run();
		}
	}
}
