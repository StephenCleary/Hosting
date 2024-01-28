using System.Diagnostics.CodeAnalysis;
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
#if NET472
	public static IServiceCollection AddWpfApplication<TApplication>(this IServiceCollection services)
#else
	public static IServiceCollection AddWpfApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services)
#endif
		where TApplication : Application =>
		services
			.AddSingleton(MarkupUtility.CreateInstance<TApplication>())
			.AddSingleton<IHostLifetime, WpfApplicationLifetime<TApplication>>();
}