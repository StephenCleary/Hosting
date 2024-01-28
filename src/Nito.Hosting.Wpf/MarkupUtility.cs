using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;

namespace Nito.Hosting.Wpf;

/// <summary>
/// Provides utilities for markup-related types.
/// </summary>
public static class MarkupUtility
{
	/// <summary>
	/// Returns a factory method for the specified type that will call <see cref="IComponentConnector.InitializeComponent"/> after creating the instance.
	/// </summary>
	/// <typeparam name="T">The markup type to be created.</typeparam>
#if NET472
	public static Func<IServiceProvider, T> CreateInstance<T>()
#else
	public static Func<IServiceProvider, T> CreateInstance<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
#endif
		where T : class =>
		provider =>
		{
			var instance = ActivatorUtilities.CreateInstance<T>(provider);
			(instance as IComponentConnector)?.InitializeComponent();
			return instance;
		};
}