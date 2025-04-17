using DWS.Console.Asynchronous;
using DWS.Console.Forms.TaskOverview;
using DWS.Console.ViewModels.TaskOverview;
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
            var viewModel = new TaskOverviewViewModel(client, supplier);
            var taskOverviewWindow = new TaskOverviewWindow(viewModel, commandProcessor, client, supplier);
            taskOverviewWindow.ShowDialog();
        });
    }
}