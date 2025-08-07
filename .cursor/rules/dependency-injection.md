---
description: Use this rule when working with dependency injection, creating new ViewModels, configuring Autofac modules, or when you need to understand how services are wired together. This rule covers DI patterns, factory methods, and lifecycle management.
---
# Dependency Injection Patterns

## Autofac Configuration

The project uses Autofac for dependency injection with custom extensions for factory pattern support.

### Module Registration
- Register modules in [ConsoleViewModelsModule.cs](mdc:DWS.Console.ViewModels/ConsoleViewModelsModule.cs)
- Use `InstancePerDependency()` for transient objects
- Use `TransitiveFactory()` for factory pattern support
- Register ViewModels with their dependencies

### Factory Pattern
- Use `TransitiveFactory()` extension method for parameter forwarding
- See [AutofacExtensions.cs](mdc:DWS.Console.ViewModels/Containers/AutofacExtensions.cs) for implementation
- This allows factory methods to receive parameters from the calling context

### Registration Patterns
```csharp
// Standard registration
builder.RegisterType<MyViewModel>()
    .AsSelf()
    .InstancePerDependency();

// Factory registration with parameter forwarding
builder.RegisterType<NewTaskViewModel>()
    .AsSelf()
    .InstancePerDependency()
    .TransitiveFactory();
```

### Dependency Resolution
- Use constructor injection for dependencies
- Use `Func<T>` for factory dependencies
- Resolve dependencies through the container in tests
- Use `Container.Resolve<T>()` for manual resolution when needed

### Test Configuration
- Use `TestModule` for test-specific registrations
- Create container in test base classes
- Resolve dependencies through the test container
- Use factory methods for creating test instances

## ViewModel Dependencies

### Common Dependencies
- `JinagaClient` - For fact operations
- `Supplier` - Current supplier context
- Factory delegates for child ViewModels

### Factory Pattern Usage
```csharp
// Registration
builder.RegisterType<YardViewModel>()
    .AsSelf()
    .InstancePerDependency();

// Usage in ViewModel
private readonly Func<Yard, YardViewModel> yardFactory;

public NewTaskViewModel(
    JinagaClient jinagaClient,
    Supplier supplier,
    Func<Yard, YardViewModel> yardFactory)
{
    this.yardFactory = yardFactory;
}
```

### Lifecycle Management
- Use `Load()` and `Unload()` methods for ViewModel lifecycle
- Implement `Ready()` method for async initialization
- Dispose observers and subscriptions in `Unload()`
- Use `Task.WhenAll()` for parallel loading operations
