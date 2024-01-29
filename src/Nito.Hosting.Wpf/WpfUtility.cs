using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;

namespace Nito.Hosting.Wpf;

/// <summary>
/// Provides utilities for WPF types.
/// </summary>
public static class WpfUtility
{
	/// <summary>
	/// Returns a factory method for the specified type that will call <see cref="IComponentConnector.InitializeComponent"/> after creating the instance.
	/// </summary>
	/// <remarks>
	///	<para>Most markup-related types invoke <c>InitializeComponent</c> from their constructor.</para>
	/// <para>For some completely unknown reason, WPF application components normally do <em>not</em>. For those components, you must explicitly call <c>InitializeComponent</c> after construction.</para>
	/// <para>Who knows why. It's just an annoying inconsistency.</para>
	/// <para>Naturally, <c>InitializeComponent</c> must be called. And cannot be called twice. And there's no way to tell whether it's been called.</para>
	/// <para>Sigh.</para>
	/// </remarks>
	/// <typeparam name="T">The WPF application type to be created.</typeparam>
#if NET472
	public static Func<IServiceProvider, T> CreateApplicationInstance<T>()
#else
	public static Func<IServiceProvider, T> CreateApplicationInstance<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>()
#endif
		where T : class =>
		provider =>
		{
			var instance = ActivatorUtilities.CreateInstance<T>(provider);
			(instance as IComponentConnector)?.InitializeComponent();
			return instance;
		};
}