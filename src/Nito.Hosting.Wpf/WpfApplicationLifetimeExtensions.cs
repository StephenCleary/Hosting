using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Nito.Hosting.Wpf;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting;

public static class WpfApplicationLifetimeHostBuilderExtensions
{
	public static IServiceCollection AddWpfApplication<TApplication>(this IServiceCollection services)
		where TApplication : Application =>
		services
			.AddSingleton<TApplication>()
			.AddSingleton<IHostLifetime, WpfApplicationLifetime<TApplication>>();
}