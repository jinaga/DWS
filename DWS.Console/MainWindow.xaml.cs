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
    private readonly IContainer container;

    public MainWindow()
    {
        // Get the container from the application
        container = ((App)Application.Current).Container;
        
        // Resolve the command processor from the container
        commandProcessor = container.Resolve<CommandProcessor>();
        
        InitializeComponent();
    }

    private void ShowTaskOverviewWindow_Click(object sender, RoutedEventArgs e)
    {
        commandProcessor.Run(async () =>
        {
            // Create and show the TaskOverviewWindow
            var taskOverviewWindow = new TaskOverviewWindow();
            taskOverviewWindow.ShowDialog();
        });
    }
}