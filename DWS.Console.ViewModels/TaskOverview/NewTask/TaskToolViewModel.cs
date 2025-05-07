namespace DWS.Console.ViewModels.TaskOverview.NewTask;

public partial class TaskToolViewModel : ObservableObject
{
    public TaskToolViewModel(Tool tool)
    {
        this.Tool = tool;
    }

    public TaskToolViewModel(string toolName)
    {
        this.Name = toolName;
    }

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private Tool? tool;
}
