namespace DWS.Console.ViewModels.TaskOverview.NewTask;

public partial class WorkerViewModel : ObservableObject
{
    public WorkerViewModel(Worker worker)
    {
        Worker = worker;
    }

    [ObservableProperty]
    private string name = string.Empty;

    public Worker Worker { get; }
}
