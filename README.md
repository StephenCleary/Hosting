![Logo](src/icon.png)

# Hosting [![Build status](https://github.com/StephenCleary/Hosting/workflows/Build/badge.svg)](https://github.com/StephenCleary/Hosting/actions?query=workflow%3ABuild) [![codecov](https://codecov.io/gh/StephenCleary/Hosting/branch/main/graph/badge.svg)](https://codecov.io/gh/StephenCleary/Hosting) [![NuGet version](https://badge.fury.io/nu/Nito.Hosting.Wpf.svg)](https://www.nuget.org/packages/Nito.Hosting.Wpf) [![API docs](https://img.shields.io/badge/API-FuGet-blue.svg)](https://www.fuget.org/packages/Nito.Hosting.Wpf)

.NET Generic Host support for UI applications.

## Usage (WPF)

Afer installing the [NuGet package](https://www.nuget.org/packages/Nito.Hosting.Wpf), add a `Main` method to your application component (commonly called `App.xaml`):

```C#
[STAThread]
private static void Main()
{
	var hostBuilder = Host.CreateApplicationBuilder();
	hostBuilder.Services.AddWpfApplication<App>();
	var host = hostBuilder.Build();
	host.RunWpfApplication<App>();
}
```

Next, disable the WPF-provided `Main` method.

For SDK-style WPF apps, set [`EnableDefaultApplicationDefinition` to `false`](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props-desktop?WT.mc_id=DT-MVP-5000058#enabledefaultapplicationdefinition) in your project's properties.

For .NET Framework WPF apps, change the build tool for your application component (usually `App.xaml`) from `Application` to `Page`.

## The .NET Generic Host and WPF Lifetime

The .NET Generic Host will shut down when the WPF application exits. Also, when the .NET Generic Host shuts down, it will request the WPF application to exit.

So, your application can exit the WPF application normally (e.g., `Application.Current.Shutdown()`), or it can request a shutdown via the .NET Generic Host (e.g., `IHostApplicationLifetime.StopApplication()`). Either of these will shut down the application cleanly.

## Using the .NET Generic Host

You can modify the `Main` method to:

- Register additional services for DI.
- Save the host and/or its `IServiceProvider` to act as a service locator.

This project provides nothing fancy like automatically injecting ViewModels into Views, or Views for ViewModels. The exact way you use DI is up to you; this project just provides the .NET Generic Host with a lifetime compatible with UI applications.

## Advanced Startup

`AddWpfApplication<App>` does two things:
- Sets the host lifetime to an instance of `WpfApplicationLifetime<App>`.
- Registers `App` as a singleton instance, creating the instance and then calling `InitializeComponent` to load the `App.xaml`.

If you need to customize the creation of your `App` instance, you can do both of the above yourself:

```C#
[STAThread]
private static void Main()
{
	var hostBuilder = Host.CreateApplicationBuilder();
	hostBuilder.Services.AddSingleton<IHostLifetime, WpfApplicationLifetime<App>>();
	hostBuilder.Services.AddSingleton<App>(provider =>
	{
		// Add any custom initialization work to this method.
		var app = new App();
		app.InitializeComponent();
		return app;

		// Do not call App.Run() - RunWpfApplication<App> will call App.Run().
	});
	var host = hostBuilder.Build();
	host.RunWpfApplication<App>();
}
```
