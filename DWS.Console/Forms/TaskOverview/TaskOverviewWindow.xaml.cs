using DWS.Console.Asynchronous;
using DWS.Console.Forms.TaskOverview.NewTask;
using DWS.Console.ViewModels.TaskOverview;
using DWS.Console.ViewModels.TaskOverview.NewTask;
using DWS.Model;
using Notifications.Wpf.ViewModels.Base;
using System.Windows;


namespace DWS.Console.Forms.TaskOverview
{
    /// <summary>
    /// Interaction logic for TaskOverviewWindow.xaml
    /// </summary>
    public partial class TaskOverviewWindow : Window
    {
        private readonly TaskOverviewViewModel taskOverviewviewModel;
        private readonly Supplier supplier;
        private readonly Func<Supplier, NewTaskDialog> newTaskDialogFactory;

        public TaskOverviewWindow(TaskOverviewViewModel viewModel, Supplier supplier,
            Func<Supplier, NewTaskDialog> newTaskDialogFactory)
        {
            this.taskOverviewviewModel = viewModel;
            this.supplier = supplier;
            this.newTaskDialogFactory = newTaskDialogFactory;
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
            var newTaskDialog = newTaskDialogFactory(supplier);
            newTaskDialog.ShowDialog();
        }


    }


}
