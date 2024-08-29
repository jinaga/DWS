using DWS.Console.Areas.Tasks;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DWS.Console;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void NewTask_Click(object sender, RoutedEventArgs e)
    {
        var client = JinagaConfig.Client;
        var supplier = await JinagaConfig.CreateSampleData(client);
        var viewModel = new NewTaskViewModel(client, supplier);
        var newTaskDialog = new NewTaskDialog(viewModel);
        newTaskDialog.ShowDialog();
    }
}