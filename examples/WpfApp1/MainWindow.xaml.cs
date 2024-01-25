using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			App.AppHost.Services.GetRequiredService<IHostApplicationLifetime>().StopApplication();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
