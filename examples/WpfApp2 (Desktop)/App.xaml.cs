using Microsoft.Extensions.Hosting;
using System.Windows;

namespace WpfApp2__Desktop_
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static IHost AppHost { get; private set; }

		private static void Main()
		{
			var hostBuilder = Host.CreateApplicationBuilder();
			hostBuilder.Services.AddWpfApplication<App>();
			AppHost = hostBuilder.Build();
			AppHost.Run();
		}
	}
}
