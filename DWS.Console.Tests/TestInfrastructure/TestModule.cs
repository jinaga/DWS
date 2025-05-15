using Autofac;
using DWS.Console.ViewModels;

namespace DWS.Console.Tests.TestInfrastructure;

public class TestModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register JinagaClient
        builder.Register(c => JinagaClient.Create())
            .AsSelf()
            .SingleInstance();
            
        // Register the ConsoleViewModelsModule to get all view model registrations
        builder.RegisterModule<ConsoleViewModelsModule>();
    }
}
