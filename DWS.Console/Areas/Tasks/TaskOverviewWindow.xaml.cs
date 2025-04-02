using DWS.Console.Asynchronous;
using DWS.Model;
using System.Windows;


namespace DWS.Console.Areas.Tasks
{
    /// <summary>
    /// Interaction logic for TaskOverviewWindow.xaml
    /// </summary>
    public partial class TaskOverviewWindow : Window
    {
        private readonly TaskOverviewViewModel taskOverviewviewModel;
        private readonly CommandProcessor commandProcessor;
        private readonly JinagaClient jinagaClient;
        private readonly Supplier supplier;


        public TaskOverviewWindow(TaskOverviewViewModel viewModel, CommandProcessor commandProcessor, JinagaClient jinagaClient, Supplier supplier)
        {
            this.taskOverviewviewModel = viewModel;
            this.commandProcessor = commandProcessor;
            this.jinagaClient = jinagaClient;
            this.supplier = supplier;
            DataContext = taskOverviewviewModel;
            InitializeComponent();
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
                var newTaskViewModel = new NewTaskViewModel(jinagaClient, supplier);
                var newTaskDialog = new NewTaskDialog(newTaskViewModel, commandProcessor);
                newTaskDialog.ShowDialog();
            });
        }
    }


}
