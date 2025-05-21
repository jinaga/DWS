using Autofac;
using DWS.Console.ViewModels.Containers;
using DWS.Console.ViewModels.TaskOverview;
using DWS.Console.ViewModels.TaskOverview.NewTask;

namespace DWS.Console.ViewModels;

public class ConsoleViewModelsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TaskOverviewViewModel>()
            .AsSelf()
            .InstancePerDependency()
            .TransitiveFactory();

        builder.RegisterType<TaskViewModel>()
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterType<NewTaskViewModel>()
            .AsSelf()
            .InstancePerDependency()
            .TransitiveFactory();
            
        // Register child view models
        builder.RegisterType<YardViewModel>()
            .AsSelf()
            .InstancePerDependency();
            
        builder.RegisterType<ToolViewModel>()
            .AsSelf()
            .InstancePerDependency();
            
        builder.RegisterType<WorkerViewModel>()
            .AsSelf()
            .InstancePerDependency();
    }
}
