using Autofac;
using DWS.Console.Asynchronous;
using DWS.Console.Forms.TaskOverview;
using DWS.Console.Forms.TaskOverview.NewTask;
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
            .SingleInstance();

        builder.RegisterType<MainWindow>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<TaskOverviewWindow>()
            .AsSelf()
            .InstancePerDependency();

        builder.RegisterType<NewTaskDialog>()
            .AsSelf()
            .InstancePerDependency();
    }
}