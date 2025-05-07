using Autofac;
using DWS.Console.ViewModels.TaskOverview;

namespace DWS.Console.ViewModels;

public class ConsoleViewModelsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TaskOverviewViewModel>()
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterType<TaskViewModel>()
            .AsSelf()
            .InstancePerDependency();
    }
}