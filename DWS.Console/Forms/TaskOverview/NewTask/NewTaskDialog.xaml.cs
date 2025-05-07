using Autofac;
using DWS.Console.Asynchronous;
using System.Windows;
using DWS.Console.ViewModels.TaskOverview.NewTask;

namespace DWS.Console.Forms.TaskOverview.NewTask
{
    /// <summary>
    /// Interaction logic for NewTaskDialog.xaml
    /// </summary>
    public partial class NewTaskDialog : Window
    {
        private readonly NewTaskViewModel viewModel;
        private readonly CommandProcessor commandProcessor;

        public NewTaskDialog()
        {
            // Get the container from the application
            var container = ((App)Application.Current).Container;
            
            // Resolve dependencies from the container
            viewModel = container.Resolve<NewTaskViewModel>();
            commandProcessor = container.Resolve<CommandProcessor>();
            
            // Set the DataContext
            DataContext = viewModel;
            
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            viewModel.Load();
            base.OnInitialized(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            viewModel.Unload();
            base.OnClosed(e);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            commandProcessor.Run(async () =>
            {
                await viewModel.Save();
                DialogResult = true;
            });
        }

    }
}
