using Autofac;
using System.Windows;

namespace DWS.Console;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IContainer? container;

    protected override void OnStartup(StartupEventArgs e)
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<ConsoleModule>();
        container = builder.Build();

        var mainWindow = container.Resolve<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (container != null)
        {
            container.Dispose();
            container = null;
        }
    }
}

