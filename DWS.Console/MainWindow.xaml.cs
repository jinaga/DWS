using Autofac;
using DWS.Console.Asynchronous;
using DWS.Console.Forms.TaskOverview;
using DWS.Model;
using Notification.Wpf;
using System.Windows;

namespace DWS.Console;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly CommandProcessor commandProcessor;
    private readonly IContainer container;

    public MainWindow()
    {
        var notificationManager = new NotificationManager();

        var client = JinagaConfig.Client;
        var builder = new ContainerBuilder();
        builder.RegisterInstance(client);
        builder.RegisterInstance(notificationManager);
        builder.RegisterModule<ConsoleModule>();
        container = builder.Build();

        commandProcessor = container.Resolve<CommandProcessor>();

        InitializeComponent();
    }

    private void ShowTaskOverviewWindow_Click(object sender, RoutedEventArgs e)
    {
        commandProcessor.Run(async () =>
        {
            var client = container.Resolve<JinagaClient>();
            var supplier = await JinagaConfig.CreateSampleData(client);

            var taskOverviewWindowFactory = container.Resolve<Func<Supplier, TaskOverviewWindow>>();
            var taskOverviewWindow = taskOverviewWindowFactory(supplier);
            taskOverviewWindow.ShowDialog();
        });
    }
}