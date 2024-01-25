using System.Windows;
using Microsoft.Extensions.Hosting;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;

namespace Nito.Hosting.Wpf;

public sealed class WpfApplicationLifetime<TApplication> : IHostLifetime
	where TApplication : Application
{
	public WpfApplicationLifetime(IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
	{
		_applicationLifetime = applicationLifetime;
		_serviceProvider = serviceProvider;
	}

	public async Task WaitForStartAsync(CancellationToken cancellationToken)
	{
		var ready = new TaskCompletionSource<TApplication>();
		cancellationToken.Register(() => ready.TrySetCanceled(cancellationToken));
		var thread = new Thread(() =>
		{
			try
			{
				var app = _serviceProvider.GetRequiredService<TApplication>();
				(app as IComponentConnector)?.InitializeComponent();
				app.Startup += (_, _) =>
				{
					ready.TrySetResult(app);
				};
				app.Exit += (_, _) =>
				{
					_applicationExited.TrySetResult(null!);
					_applicationLifetime.StopApplication();
				};
				Environment.ExitCode = app.Run();
			}
			catch (Exception ex)
			{
				if (ready.TrySetException(ex))
					return;
				throw;
			}
		});
		thread.Name = "Main WPF Thread";
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		_application = await ready.Task;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_ = _application ?? throw new InvalidOperationException($"{nameof(StopAsync)} invoked before {nameof(WaitForStartAsync)} completed.");
		_application.Dispatcher.BeginInvoke(() => _application.Shutdown());
		return _applicationExited.Task;
	}

	private readonly IHostApplicationLifetime _applicationLifetime;
	private readonly IServiceProvider _serviceProvider;
	private readonly TaskCompletionSource<object> _applicationExited = new();
	private TApplication? _application;
}