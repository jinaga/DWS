using Autofac;
using DWS.Console.Asynchronous;
using DWS.Console.ViewModels.TaskOverview;
using DWS.Console.ViewModels.TaskOverview.NewTask;
using DWS.Model;
using Jinaga;
using System;
using System.Windows;

namespace DWS.Console;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IContainer? container;
    
    // Add a public property to access the container
    public IContainer Container => container ?? throw new InvalidOperationException("Container not initialized");

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Build the Autofac container
        var builder = new ContainerBuilder();
        
        // Register core dependencies
        var jinagaClient = JinagaConfig.CreateJinagaClient();
        builder.RegisterInstance(jinagaClient).As<JinagaClient>().SingleInstance();
        
        // Register the supplier (assuming it's available from somewhere)
        var supplier = GetSupplier(jinagaClient);
        builder.RegisterInstance(supplier).AsSelf().SingleInstance();
        
        // Register services
        builder.RegisterType<CommandProcessor>().AsSelf().SingleInstance();
        
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
        
        // Build the container
        container = builder.Build();
        
        // Create and show the main window
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
    
    private Supplier GetSupplier(JinagaClient jinagaClient)
    {
        // Use the existing method to create sample data
        return JinagaConfig.CreateSampleData(jinagaClient).Result;
    }
}

