using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AvaloniaModel.ViewModels;
using AvaloniaModel.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using AvaloniaModel.Services;

namespace AvaloniaModel;

public partial class App : Application
{
    public static IServiceCollection services { get; set; }
    public static IServiceProvider Provider { get; private set; }
    public override void Initialize()
    {
        services = new ServiceCollection();
        services.AddSingleton<MessageBoxService>();

        AvaloniaXamlLoader.Load(this);
        Provider = services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}