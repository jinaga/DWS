using DWS.Model;

namespace DWS.Console.Areas.Tasks;

public partial class TaskToolViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private Tool? tool;
}
