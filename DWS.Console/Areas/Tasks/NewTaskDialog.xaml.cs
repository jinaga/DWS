using DWS.Console.Asynchronous;
using Notifications.Wpf;
using System.Windows;

namespace DWS.Console.Areas.Tasks
{
    /// <summary>
    /// Interaction logic for NewTaskDialog.xaml
    /// </summary>
    public partial class NewTaskDialog : Window
    {
        private readonly NewTaskViewModel viewModel;
        private readonly CommandProcessor commandProcessor;

        public NewTaskDialog(NewTaskViewModel viewModel, CommandProcessor commandProcessor)
        {
            this.viewModel = viewModel;
            this.commandProcessor = commandProcessor;
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
                await Task.Delay(400);
                throw new Exception("Test exception");
                //await viewModel.Save();
                //DialogResult = true;
            });
        }
    }
}
