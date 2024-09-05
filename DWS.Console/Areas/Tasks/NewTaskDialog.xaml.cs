using System.Windows;

namespace DWS.Console.Areas.Tasks
{
    /// <summary>
    /// Interaction logic for NewTaskDialog.xaml
    /// </summary>
    public partial class NewTaskDialog : Window
    {
        private readonly NewTaskViewModel viewModel;

        public NewTaskDialog(NewTaskViewModel viewModel)
        {
            this.viewModel = viewModel;
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

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.Save();
            DialogResult = true;
        }
    }
}
