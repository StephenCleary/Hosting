using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Nito.Hosting.Wpf;

/// <summary>
/// Provides an <see cref="IHostLifetime"/> implementation that manages the lifetime of a WPF <see cref="Application"/>.
/// The <typeparamref name="TApplication"/> instance is created during startup and shut down when the host is stopped.
/// </summary>
/// <typeparam name="TApplication">The type of WPF <see cref="Application"/> to manage.</typeparam>
public sealed class WpfApplicationLifetime<TApplication> : IHostLifetime
	where TApplication : Application
{
	/// <summary>
	/// Initializes a new instance of the <see cref="WpfApplicationLifetime{TApplication}"/> class.
	/// </summary>
	public WpfApplicationLifetime(IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
	{
		_applicationLifetime = applicationLifetime;
		_serviceProvider = serviceProvider;
	}

	/// <inheritdoc />
	public async Task WaitForStartAsync(CancellationToken cancellationToken)
	{
		var ready = new TaskCompletionSource<TApplication>();
		var thread = new Thread(() =>
		{
			using var registration = cancellationToken.Register(() => ready.TrySetCanceled(cancellationToken));
			try
			{
				var app = _serviceProvider.GetRequiredService<TApplication>();
				app.Startup += (_, _) =>
				{
					ready.TrySetResult(app);
				};
				app.Exit += (_, _) =>
				{
					_applicationExited.TrySetResult(null!);
					_applicationLifetime.StopApplication();
				};
				registration.Dispose();
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
		_application = await ready.Task.ConfigureAwait(false);
	}

	/// <inheritdoc />
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