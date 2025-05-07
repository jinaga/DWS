using DWS.Model;
using Jinaga;

namespace DWS.Console.ViewModels.TaskOverview.NewTask;

public class TaskToolViewModelFactory : ITaskToolViewModelFactory
{
    private readonly JinagaClient jinagaClient;

    public TaskToolViewModelFactory(JinagaClient jinagaClient)
    {
        this.jinagaClient = jinagaClient;
    }

    public TaskToolViewModel Create(Tool tool)
    {
        return new TaskToolViewModel(tool);
    }

    public TaskToolViewModel Create(string toolName)
    {
        return new TaskToolViewModel(toolName);
    }
}