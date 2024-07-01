using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Nito.Hosting.AvaloniauiDesktop
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static class AvaloniauiApplicationLifetimeExtensions
    {
        /// <summary>
        /// Configures the host to use avaloniaui application lifetime.
        /// Also configures the <typeparamref name="TApplication"/> as a singleton.
        /// </summary>
        /// <typeparam name="TApplication">The type of avaloniaui application <see cref="Application"/> to manage.</typeparam>
        /// <param name="appBuilderResolver"><see cref="AppBuilder.Configure{TApplication}()"/></param>
        /// <param name="commandArgs">commmandline args</param>

        public static IServiceCollection AddAvaloniauiDesktopApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services,
            Func<string[], AppBuilder> appBuilderResolver,
            Func<AppBuilder, AppBuilder> appBuilderConfiger,
            string[] commandArgs)
            where TApplication : Application, new()
        {
            var appBuilder = appBuilderResolver(commandArgs);
            appBuilder = appBuilderConfiger(appBuilder);
            return services
                    .AddSingleton(appBuilder)
                    .AddSingleton(provider =>
                    {
                        appBuilder.Instance!.Initialize();
                        return (appBuilder.Instance! as TApplication)!;
                    })
                    .AddSingleton<IHostLifetime, AvaloniauiApplicationLifetime<TApplication>>();
        }

        /// <summary>
        /// an overload without commmandargs of AddAvaloniauiDesktopApplication()
        /// </summary>
        public static IServiceCollection AddAvaloniauiDesktopApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services,
            Func<AppBuilder> appBuilderResolver)
            where TApplication : Application, new()
        {
            return services.AddAvaloniauiDesktopApplication<TApplication>(appBuilderResolver: (args) => appBuilderResolver(),
                appBuilderConfiger: appbuilder => appbuilder,
                commandArgs: Array.Empty<string>());
        }

        /// <summary>
        /// an overload without appBuilderConfiger of AddAvaloniauiDesktopApplication()
        /// </summary>
        public static IServiceCollection AddAvaloniauiDesktopApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services,
           Func<string[], AppBuilder> appBuilderResolver,
           string[] commandArgs)
           where TApplication : Application, new()
        {
            return services.AddAvaloniauiDesktopApplication<TApplication>(appBuilderResolver: (args) => appBuilderResolver(args),
                appBuilderConfiger: appbuilder => appbuilder,
                commandArgs: commandArgs);
        }

        /// <summary>
        /// an overload without appBuilderResolver and commmandargs of AddAvaloniauiDesktopApplication()
        /// </summary>
        public static IServiceCollection AddAvaloniauiDesktopApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services,
            Func<AppBuilder, AppBuilder> appBuilderConfiger)
            where TApplication : Application, new()
        {
            return services.AddAvaloniauiDesktopApplication<TApplication>(appBuilderResolver: (args) => BuildAvaloniaAppDefault<TApplication>(Array.Empty<string>()),
                appBuilderConfiger: appbuilder => appBuilderConfiger(appbuilder),
                commandArgs: Array.Empty<string>());
        }

        /// <summary>
        /// an overload without commmandargs,appBuilderResolver,appBuilderConfiger of AddAvaloniauiDesktopApplication()
        /// </summary>
        public static IServiceCollection AddAvaloniauiDesktopApplication<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication>(this IServiceCollection services)
            where TApplication : Application, new()
        {
            return services.AddAvaloniauiDesktopApplication<TApplication>(appBuilderResolver: args => BuildAvaloniaAppDefault<TApplication>(Array.Empty<string>()),
                appBuilderConfiger: appbuilder => appbuilder,
                commandArgs: Array.Empty<string>());
        }


        /// <summary>
        /// Runs the avaloniaui application along with the .NET generic host.
        /// </summary>
        /// <typeparam name="TApplication">The type of the avaloniaui application <see cref="Application"/> to run.</typeparam>
        public static void RunAvaliauiApplication<TApplication>(this IHost host,
            CancellationToken cancellationToken = default)
            where TApplication : Application
        {
            _ = host ?? throw new ArgumentNullException(nameof(host));
            var app = host.Services.GetRequiredService<TApplication>();
            var hostTask = host.RunAsync(token: cancellationToken);

            if (app.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime classicDesktop)
            {
                Environment.ExitCode = classicDesktop.Start(classicDesktop.Args ?? Array.Empty<string>());
            }
            else
            {
                throw new NotImplementedException("Genric host support classic desktop only!");
            }
            hostTask.GetAwaiter().GetResult();
        }

        private static AppBuilder BuildAvaloniaAppDefault<TApplication>(string[] args) where TApplication : Application, new()
        {
            return AppBuilder.Configure<TApplication>()
                            .UsePlatformDetect()
                            .LogToTrace()
                            .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime
                            {
                                Args = args,
                                ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose,
                            });
        }
    }
}
