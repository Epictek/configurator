using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using OpenIPC_Configurator.ViewModels;
using OpenIPC_Configurator.Views;
using Serilog;
using Logger = Serilog.Core.Logger;

namespace OpenIPC_Configurator;

public partial class App : Application
{
    public static Logger Logger = new LoggerConfiguration()
        .WriteTo.Console()
    .CreateLogger();

    public override void Initialize()
    {
        
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}