using Autofac;
using DWS.Console.Asynchronous;
using DWS.Console.Forms.TaskOverview;
using Notification.Wpf;
using System.Windows;

namespace DWS.Console;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly CommandProcessor commandProcessor;

    public MainWindow()
    {
        var notificationManager = new NotificationManager();
        commandProcessor = new CommandProcessor(notificationManager);
        InitializeComponent();
    }

    private void ShowTaskOverviewWindow_Click(object sender, RoutedEventArgs e)
    {
        commandProcessor.Run(async () =>
        {
            var client = JinagaConfig.Client;
            var supplier = await JinagaConfig.CreateSampleData(client);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(client);
            builder.RegisterInstance(supplier);
            builder.RegisterInstance(commandProcessor);
            builder.RegisterModule<ConsoleModule>();
            var container = builder.Build();

            var taskOverviewWindow = container.Resolve<TaskOverviewWindow>();
            taskOverviewWindow.ShowDialog();
        });
    }
}