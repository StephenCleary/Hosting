using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using Nito.Hosting.Wpf;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for configuring WPF application lifetimes.
/// </summary>
public static class WpfApplicationLifetimeHostBuilderExtensions
{
	/// <summary>
	/// Configures the host to use WPF application lifetime.
	/// Also configures the <typeparamref name="TApplication"/> as a singleton.
	/// </summary>
	/// <typeparam name="TApplication">The type of WPF <see cref="Application"/> to manage.</typeparam>
#if NET472
	public static IServiceCollection AddWpfApplication<TApplication>(this IServiceCollection services)
#else
	public static IServiceCollection AddWpfApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services)
#endif
		where TApplication : Application =>
		services
			.AddSingleton(provider =>
			{
				// Most markup-related types invoke InitializeComponent from their constructor.
				// For some completely unknown reason, WPF application components normally do *not*. For those components, you must explicitly call InitializeComponent after construction.
				// Who knows why. It's just an annoying inconsistency.
				// Naturally, InitializeComponent *must* be called. And must *not* be called twice. And there's no way to tell whether it's been called.
				// Sigh.
				var instance = ActivatorUtilities.CreateInstance<TApplication>(provider);
				(instance as IComponentConnector)?.InitializeComponent();
				return instance;
			})
			.AddSingleton<IHostLifetime, WpfApplicationLifetime<TApplication>>();
}