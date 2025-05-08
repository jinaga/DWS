using Autofac;
using DWS.Console.Asynchronous;
using DWS.Console.Forms.TaskOverview;
using DWS.Console.ViewModels;
using Notification.Wpf;

namespace DWS.Console;

class ConsoleModule: Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<ConsoleViewModelsModule>();

        builder.Register(c => JinagaConfig.CreateJinagaClient())
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<NotificationManager>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<CommandProcessor>()
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterType<MainWindow>()
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterType<TaskOverviewWindow>()
            .AsSelf()
            .InstancePerDependency();
    }
}