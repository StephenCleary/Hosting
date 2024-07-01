using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.Hosting;
using System.Runtime.Versioning;

namespace Nito.Hosting.AvaloniauiDesktop
{
    /// <summary>
    /// Provides an <see cref="IHostLifetime"/> implementation that manages the lifetime of a Avaloniaui classic desktop <see cref="Application"/> <see cref="IControlledApplicationLifetime"/>.
    /// The <typeparamref name="TApplication"/> instance is created during startup and shut down when the host is stopped.
    /// </summary>
    /// <typeparam name="TApplication">The type of Avaloniaui Application <see cref="Application"/> to manage.</typeparam>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public sealed class AvaloniauiApplicationLifetime<TApplication> : IHostLifetime
        where TApplication : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniauiApplicationLifetime{TApplication}"/> class.
        /// </summary>
        public AvaloniauiApplicationLifetime(IHostApplicationLifetime applicationLifetime, TApplication application)
        {
            _applicationLifetime = applicationLifetime;
            _application = application;
        }

        /// <inheritdoc />
        public async Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            var ready = new TaskCompletionSource<object>();
            using var registration = cancellationToken.Register(() => ready.TrySetCanceled(cancellationToken));

            if (_application.ApplicationLifetime is IControlledApplicationLifetime desktopLifetime)
            {
                desktopLifetime.Startup += (_, _) => ready.TrySetResult(null!);
                desktopLifetime.Exit += (_, _) =>
                {
                    _applicationExited.TrySetResult(null!);
                    _applicationLifetime.StopApplication();
                };
            }
            else
            {
                ready.TrySetException(new NotImplementedException("Genric host support classic desktop only!"));
            }
            await ready.Task.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispatcher.UIThread.BeginInvokeShutdown(DispatcherPriority.Default);
            return _applicationExited.Task;
        }

        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly TaskCompletionSource<object> _applicationExited = new();
        private readonly TApplication _application;
    }
}
