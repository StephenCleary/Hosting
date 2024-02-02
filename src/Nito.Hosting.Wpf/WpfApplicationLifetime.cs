using System.Windows;
using Microsoft.Extensions.Hosting;

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
	public WpfApplicationLifetime(IHostApplicationLifetime applicationLifetime, TApplication application)
	{
		_applicationLifetime = applicationLifetime;
		_application = application;
	}

	/// <inheritdoc />
	public async Task WaitForStartAsync(CancellationToken cancellationToken)
	{
		var ready = new TaskCompletionSource<object>();
		using var registration = cancellationToken.Register(() => ready.TrySetCanceled(cancellationToken));
		_application.Startup += (_, _) => ready.TrySetResult(null!);
		_application.Exit += (_, _) =>
		{
			_applicationExited.TrySetResult(null!);
			_applicationLifetime.StopApplication();
		};
		await ready.Task.ConfigureAwait(false);
	}

	/// <inheritdoc />
	public Task StopAsync(CancellationToken cancellationToken)
	{
		_application.Dispatcher.BeginInvoke(() => _application.Shutdown());
		return _applicationExited.Task;
	}

	private readonly IHostApplicationLifetime _applicationLifetime;
	private readonly TaskCompletionSource<object> _applicationExited = new();
	private readonly TApplication _application;
}