using DWS.Model;

namespace DWS.Console.ViewModels.TaskOverview.NewTask;

public interface ITaskToolViewModelFactory
{
    TaskToolViewModel Create(Tool tool);
    TaskToolViewModel Create(string toolName);
}