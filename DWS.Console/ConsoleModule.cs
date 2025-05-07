using Autofac;
using DWS.Console.Asynchronous;
using DWS.Console.Forms.TaskOverview;
using DWS.Console.ViewModels;

namespace DWS.Console;

class ConsoleModule: Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<ConsoleViewModelsModule>();

        builder.RegisterType<TaskOverviewWindow>()
            .AsSelf()
            .InstancePerDependency();
    }
}