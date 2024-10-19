using DWS.Model;

namespace DWS.Console.Areas.Tasks;

public partial class ToolViewModel : ObservableObject
{
    public ToolViewModel(Tool tool)
    {
        Tool = tool;
    }

    [ObservableProperty]
    private string name = string.Empty;

    public Tool Tool { get; }
}
