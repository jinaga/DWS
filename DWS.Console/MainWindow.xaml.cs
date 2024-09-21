using DWS.Console.Areas.Tasks;
using DWS.Console.Asynchronous;
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

    private void NewTask_Click(object sender, RoutedEventArgs e)
    {
        commandProcessor.Run(async () =>
        {
            var client = JinagaConfig.Client;
            var supplier = await JinagaConfig.CreateSampleData(client);
            var viewModel = new NewTaskViewModel(client, supplier);
            var newTaskDialog = new NewTaskDialog(viewModel, commandProcessor);
            newTaskDialog.ShowDialog();
        });
    }
}