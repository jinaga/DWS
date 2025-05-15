# Dependency Injection Review

## Overview

This document outlines the findings from a review of the DWS project, focusing on opportunities to improve dependency injection usage. The project already uses Autofac for dependency injection, but there are several instances where objects are created directly rather than being injected or created via factory methods.

## Current DI Implementation

The project uses Autofac for dependency injection with:
- `ConsoleModule` - Registers UI components and services
- `ConsoleViewModelsModule` - Registers view models
- `TransitiveFactory` extension - Custom extension for parameter forwarding

## Instances of Direct Object Creation

### 1. View Model Factory Methods

Several view models directly create child view models:

```csharp
// In NewTaskViewModel.LoadYards()
YardViewModel yard = new YardViewModel(yardProjection.yard);

// In NewTaskViewModel.LoadTools()
ToolViewModel tool = new ToolViewModel(toolProjection.tool);

// In NewTaskViewModel.LoadWorkers()
WorkerViewModel worker = new WorkerViewModel(workerProjection.worker);
```

### 2. JinagaClient Creation

```csharp
// In JinagaConfig.CreateJinagaClient()
JinagaClient jinagaClient = JinagaClient.Create();
```

### 3. Domain Model Creation

```csharp
// In NewTaskViewModel.Save()
var client = await jinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
var yard = await jinagaClient.Fact(new Yard(client, Guid.NewGuid()));
var task = await jinagaClient.Fact(new DWSTask(selectedYard, Guid.NewGuid()));
```

### 4. Notification Content

```csharp
// In CommandProcessor.Run()
notificationManager.Show(new NotificationContent { ... });
```

## Recommendations Checklist

### 1. Register Child View Models in DI Container

- [ ] Register YardViewModel in the DI container
- [ ] Register ToolViewModel in the DI container
- [ ] Register WorkerViewModel in the DI container
- [ ] Inject factories for these view models into NewTaskViewModel
- [ ] Update NewTaskViewModel to use the injected factories

### 2. Create a JinagaClientFactory

- [ ] Create IJinagaClientFactory interface
- [ ] Implement DefaultJinagaClientFactory
- [ ] Register the factory in the DI container
- [ ] Update JinagaConfig to use the factory

### 3. Create Domain Model Factories

- [ ] Create IClientFactory interface
- [ ] Create IYardFactory interface
- [ ] Create ITaskFactory interface
- [ ] Implement JinagaClientFactory class that implements these interfaces
- [ ] Register the factories in the DI container
- [ ] Inject the factories into NewTaskViewModel
- [ ] Update NewTaskViewModel.Save() to use the factories

### 4. Create a NotificationFactory

- [ ] Create INotificationFactory interface
- [ ] Implement DefaultNotificationFactory
- [ ] Register the factory in the DI container
- [ ] Inject the factory into CommandProcessor
- [ ] Update CommandProcessor.Run() to use the factory

## Implementation Details

### 1. Register Child View Models

```csharp
// In ConsoleViewModelsModule.Load
builder.RegisterType<YardViewModel>()
    .AsSelf()
    .InstancePerDependency();

builder.RegisterType<ToolViewModel>()
    .AsSelf()
    .InstancePerDependency();

builder.RegisterType<WorkerViewModel>()
    .AsSelf()
    .InstancePerDependency();
```

Then inject factories into NewTaskViewModel:

```csharp
public NewTaskViewModel(
    JinagaClient jinagaClient, 
    Supplier supplier,
    Func<Yard, YardViewModel> yardFactory,
    Func<Tool, ToolViewModel> toolFactory,
    Func<Worker, WorkerViewModel> workerFactory)
{
    this.jinagaClient = jinagaClient;
    this.supplier = supplier;
    this.yardFactory = yardFactory;
    this.toolFactory = toolFactory;
    this.workerFactory = workerFactory;
}
```

And use them in the Load methods:

```csharp
// In LoadYards()
YardViewModel yard = yardFactory(yardProjection.yard);

// In LoadTools()
ToolViewModel tool = toolFactory(toolProjection.tool);

// In LoadWorkers()
WorkerViewModel worker = workerFactory(workerProjection.worker);
```

### 2. JinagaClientFactory

```csharp
public interface IJinagaClientFactory
{
    JinagaClient Create();
}

public class DefaultJinagaClientFactory : IJinagaClientFactory
{
    public JinagaClient Create()
    {
        return JinagaClient.Create();
    }
}
```

Register it in the DI container:

```csharp
// In ConsoleModule.Load
builder.RegisterType<DefaultJinagaClientFactory>()
    .As<IJinagaClientFactory>()
    .SingleInstance();

builder.Register(c => c.Resolve<IJinagaClientFactory>().Create())
    .AsSelf()
    .SingleInstance();
```

Update JinagaConfig:

```csharp
static class JinagaConfig
{
    public static JinagaClient CreateJinagaClient(IJinagaClientFactory factory)
    {
        return factory.Create();
    }
    
    // ...
}
```

### 3. Domain Model Factories

```csharp
public interface IClientFactory
{
    Task<Client> CreateClient(Supplier supplier, string name);
}

public interface IYardFactory
{
    Task<Yard> CreateYard(Client client, string name);
}

public interface ITaskFactory
{
    Task<DWSTask> CreateTask(Yard yard);
}
```

With implementations:

```csharp
public class JinagaModelFactory : IClientFactory, IYardFactory, ITaskFactory
{
    private readonly JinagaClient jinagaClient;

    public JinagaModelFactory(JinagaClient jinagaClient)
    {
        this.jinagaClient = jinagaClient;
    }

    public async Task<Client> CreateClient(Supplier supplier, string name)
    {
        var client = await jinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new ClientName(client, name, []));
        return client;
    }

    public async Task<Yard> CreateYard(Client client, string name)
    {
        var yard = await jinagaClient.Fact(new Yard(client, Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yard, name, []));
        return yard;
    }

    public async Task<DWSTask> CreateTask(Yard yard)
    {
        return await jinagaClient.Fact(new DWSTask(yard, Guid.NewGuid()));
    }
}
```

Register these in the DI container:

```csharp
// In ConsoleModule.Load
builder.RegisterType<JinagaModelFactory>()
    .As<IClientFactory>()
    .As<IYardFactory>()
    .As<ITaskFactory>()
    .SingleInstance();
```

### 4. NotificationFactory

```csharp
public interface INotificationFactory
{
    NotificationContent CreateErrorNotification(string title, string message);
}

public class DefaultNotificationFactory : INotificationFactory
{
    public NotificationContent CreateErrorNotification(string title, string message)
    {
        return new NotificationContent
        {
            Title = title,
            Message = message,
            Type = NotificationType.Error
        };
    }
}
```

Register and inject into CommandProcessor:

```csharp
// In ConsoleModule.Load
builder.RegisterType<DefaultNotificationFactory>()
    .As<INotificationFactory>()
    .SingleInstance();

// Update CommandProcessor
public CommandProcessor(NotificationManager notificationManager, INotificationFactory notificationFactory)
{
    this.notificationManager = notificationManager;
    this.notificationFactory = notificationFactory;
}

// In Run method
if (t.IsFaulted)
{
    var notification = notificationFactory.CreateErrorNotification(
        "Error while saving", 
        t.Exception?.Message);
    notificationManager.Show(notification);
}
```

## Benefits of These Changes

1. **Improved Testability**: Dependencies can be easily mocked in unit tests
2. **Better Separation of Concerns**: Each class has clear responsibilities
3. **Flexibility**: Implementation details can be changed without modifying consumers
4. **Consistency**: Follows a uniform pattern for object creation throughout the application
