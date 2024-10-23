using DWS.Model;

namespace DWS.Console.ViewModels.Tasks;

public partial class TaskToolViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private Tool? tool;
}
