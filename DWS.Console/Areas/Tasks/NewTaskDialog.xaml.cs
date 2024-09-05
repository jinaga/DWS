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

        protected override void OnActivated(EventArgs e)
        {
            viewModel.Load();
            base.OnActivated(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            viewModel.Unload();
            base.OnDeactivated(e);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
