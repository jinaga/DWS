using DWS.Console.Asynchronous;
using DWS.Console.Forms.TaskOverview;
using DWS.Model;
using System.Windows;

namespace DWS.Console;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly CommandProcessor commandProcessor;
    private readonly JinagaClient client;
    private readonly Func<Supplier, TaskOverviewWindow> taskOverviewWindowFactory;

    public MainWindow(CommandProcessor commandProcessor, JinagaClient client, Func<Supplier, TaskOverviewWindow> taskOverviewWindowFactory)
    {
        this.commandProcessor = commandProcessor;
        this.client = client;
        this.taskOverviewWindowFactory = taskOverviewWindowFactory;
        InitializeComponent();
    }

    private void ShowTaskOverviewWindow_Click(object sender, RoutedEventArgs e)
    {
        commandProcessor.Run(async () =>
        {
            var supplier = await JinagaConfig.CreateSampleData(client);
            var taskOverviewWindow = taskOverviewWindowFactory(supplier);
            taskOverviewWindow.ShowDialog();
        });
    }
}