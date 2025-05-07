using Autofac;
using DWS.Console.ViewModels.TaskOverview;
using DWS.Console.ViewModels.TaskOverview.NewTask;
using DWS.Model;
using Jinaga;
using Jinaga.UnitTest;
using System;

namespace DWS.Console.Tests
{
    public class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Create a Jinaga client for testing
            var jinagaClient = JinagaTest.Create();
            builder.RegisterInstance(jinagaClient).As<JinagaClient>().SingleInstance();
            
            // Register view models
            builder.RegisterType<TaskOverviewViewModel>().AsSelf();
            builder.RegisterType<TaskViewModel>().AsSelf();
            builder.RegisterType<NewTaskViewModel>().AsSelf();
            builder.RegisterType<YardViewModel>().AsSelf();
            builder.RegisterType<ToolViewModel>().AsSelf();
            builder.RegisterType<WorkerViewModel>().AsSelf();
            builder.RegisterType<TaskToolViewModel>().AsSelf();
            
            // Register factories
            builder.RegisterType<TaskToolViewModelFactory>().As<ITaskToolViewModelFactory>();
            
            // Register factory methods for view models
            builder.Register<Func<DWSTask, TaskViewModel>>(c => 
                (task) => new TaskViewModel(c.Resolve<JinagaClient>(), task));
                
            builder.Register<Func<Yard, YardViewModel>>(c => 
                (yard) => new YardViewModel(yard));
                
            builder.Register<Func<Tool, ToolViewModel>>(c => 
                (tool) => new ToolViewModel(tool));
                
            builder.Register<Func<Worker, WorkerViewModel>>(c => 
                (worker) => new WorkerViewModel(worker));
        }
    }
}