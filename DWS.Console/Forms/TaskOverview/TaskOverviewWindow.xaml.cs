using DWS.Console.Asynchronous;
using DWS.Console.Forms.TaskOverview.NewTask;
using DWS.Console.ViewModels.TaskOverview;
using DWS.Console.ViewModels.TaskOverview.NewTask;
using System.Windows;

namespace DWS.Console.Forms.TaskOverview
{
    /// <summary>
    /// Interaction logic for TaskOverviewWindow.xaml
    /// </summary>
    public partial class TaskOverviewWindow : Window
    {
        private readonly TaskOverviewViewModel taskOverviewviewModel;
        private readonly CommandProcessor commandProcessor;
        private readonly IContainer container;

        public TaskOverviewWindow()
        {
            InitializeComponent();
            
            // Get the container from the application
            container = ((App)Application.Current).Container;
            
            // Get the command processor
            commandProcessor = container.Resolve<CommandProcessor>();
            
            // Resolve the view model from the container
            taskOverviewviewModel = container.Resolve<TaskOverviewViewModel>();
            
            // Set the DataContext
            DataContext = taskOverviewviewModel;
        }

        protected override void OnInitialized(EventArgs e)
        {
            taskOverviewviewModel.Load();
            base.OnInitialized(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            taskOverviewviewModel.Unload();
            base.OnClosed(e);
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            commandProcessor.Run(async () =>
            {
                // Resolve the NewTaskViewModel from the container
                var newTaskViewModel = container.Resolve<NewTaskViewModel>();
                
                // Create the dialog with the resolved view model
                var newTaskDialog = new NewTaskDialog();
                newTaskDialog.DataContext = newTaskViewModel;
                newTaskDialog.ShowDialog();
            });
        }


    }


}
