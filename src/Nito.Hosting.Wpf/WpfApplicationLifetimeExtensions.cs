using System.Windows;
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
	public static IServiceCollection AddWpfApplication<TApplication>(this IServiceCollection services)
		where TApplication : Application =>
		services
			.AddSingleton<TApplication>()
			.AddSingleton<IHostLifetime, WpfApplicationLifetime<TApplication>>();
}