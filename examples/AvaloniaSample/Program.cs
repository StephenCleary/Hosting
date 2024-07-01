using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nito.Hosting.AvaloniauiDesktop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Versioning;

namespace AvaloniaSample
{
    internal sealed class Program
    {
        [STAThread]
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        [SupportedOSPlatform("macos")]
        [RequiresDynamicCode("Calls Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder()")]
        public static void Main(string[] args)
        {
            var hostBuilder = Host.CreateApplicationBuilder();

            // ioc configure
            hostBuilder.Configuration.AddCommandLine(args);

            // build with args -> config
            hostBuilder.Services.AddAvaloniauiDesktopApplication<App>(BuildAvaloniaApp, ConfigAvaloniaApp, args);
            // config
            //hostBuilder.Services.AddAvaloniauiDesktopApplication<App>(ConfigAvaloniaApp);
            // build without args
            //hostBuilder.Services.AddAvaloniauiDesktopApplication<App>(BuildAvaloniaAppWithoutArgs);
            // build default
            //hostBuilder.Services.AddAvaloniauiDesktopApplication<App>();

            // build host
            var appHost = hostBuilder.Build();

            // run app
            appHost.RunAvaliauiApplication<App>();
        }

        public static AppBuilder BuildAvaloniaApp(string[] args)
        { 
            return AppBuilder.Configure<App>()
                            .UsePlatformDetect()
                            .WithInterFont()
                            .LogToTrace()
                            .UseReactiveUI()
                            .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime
                            {
                                Args = args,
                                ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose,
                            });
        }

        public static AppBuilder BuildAvaloniaAppWithoutArgs()
        {
            return AppBuilder.Configure<App>()
                            .UsePlatformDetect()
                            .WithInterFont()
                            .LogToTrace()
                            .UseReactiveUI()
                            .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime
                            {
                                ShutdownMode = Avalonia.Controls.ShutdownMode.OnMainWindowClose,
                            });
        }

        public static AppBuilder ConfigAvaloniaApp(AppBuilder appBuilder)
        {
            return appBuilder.WithInterFont().UseReactiveUI();
        }
    }
}
