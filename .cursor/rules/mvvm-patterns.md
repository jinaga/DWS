---
description: Use this rule when creating or modifying ViewModels, implementing UI interactions, handling property changes, or when you need to understand MVVM patterns. This rule covers ViewModel structure, lifecycle management, and UI binding patterns.
---
# MVVM Patterns

## ViewModel Structure

### Base Class
All ViewModels inherit from `ObservableObject` for property change notifications:

```csharp
public partial class NewTaskViewModel : ObservableObject
{
    // Properties and methods
}
```

### Property Patterns
Use `[ObservableProperty]` attribute for automatic property generation:

```csharp
[ObservableProperty]
private string clientName = string.Empty;

[ObservableProperty]
private WorkerViewModel? selectedWorker;
```

### Collections
Use `ObservableCollection<T>` for collections that need UI updates:

```csharp
public ObservableCollection<YardViewModel> Yards { get; } = [];
public ObservableCollection<ToolViewModel> ToolCatalog { get; } = [];
```

## Lifecycle Management

### Loading Pattern
Implement `Load()` and `Ready()` methods for async initialization:

```csharp
public void Load()
{
    if (yardObserver != null)
    {
        return;
    }
    
    LoadYards();
    LoadTools();
    LoadWorkers();
}

public Task Ready()
{
    return Task.WhenAll(
        yardObserver?.Loaded ?? Task.CompletedTask,
        toolObserver?.Loaded ?? Task.CompletedTask,
        workerObserver?.Loaded ?? Task.CompletedTask
    );
}
```

### Unloading Pattern
Implement `Unload()` for cleanup:

```csharp
public void Unload()
{
    UnloadYards();
    UnloadTools();
    UnloadWorkers();
}
```

## Data Loading

### Observer Pattern
Use observers for reactive data loading:

```csharp
private IObserver? yardObserver;
private IObserver? toolObserver;
private IObserver? workerObserver;

private void LoadYards()
{
    var yardsInSupplier = Given<Supplier>.Match((supplier, facts) =>
        from yard in facts.OfType<Yard>()
        where yard.supplier == supplier && !yard.IsDeleted
        // ... more query logic
    );
    
    yardObserver = jinagaClient.Watch(yardsInSupplier, yards =>
    {
        // Update collection
    });
}
```

### Collection Updates
Update collections in observer callbacks:

```csharp
yardObserver = jinagaClient.Watch(yardsInSupplier, yards =>
{
    Yards.Clear();
    foreach (var yard in yards)
    {
        Yards.Add(yardFactory(yard));
    }
});
```

## Property Change Handling

### Partial Methods
Use partial methods for custom property change logic:

```csharp
partial void OnSelectedYardChanged(YardViewModel? value)
{
    if (value != null)
    {
        // Handle selection change
    }
}
```

### Computed Properties
Use relations for computed properties that depend on other data:

```csharp
public string DisplayName => $"{FirstName} {LastName}";
```

## Command Patterns

### Async Commands
Use `AsyncRelayCommand` for async operations:

```csharp
public AsyncRelayCommand SaveCommand { get; }

public NewTaskViewModel()
{
    SaveCommand = new AsyncRelayCommand(Save, CanSave);
}

private async Task Save()
{
    // Save logic
}

private bool CanSave()
{
    // Validation logic
    return !string.IsNullOrEmpty(ClientName);
}
```

### Command Execution
Execute commands with proper error handling:

```csharp
public async Task Save()
{
    try
    {
        // Save logic
    }
    catch (Exception ex)
    {
        // Handle error
    }
}
```

## Validation

### Property Validation
Implement validation in property setters or commands:

```csharp
private bool CanSave()
{
    return !string.IsNullOrEmpty(ClientName) &&
           !string.IsNullOrEmpty(YardName) &&
           SelectedWorker != null;
}
```

### Error Handling
Use try-catch blocks for async operations:

```csharp
try
{
    await jinagaClient.Fact(new Task(...));
}
catch (Exception ex)
{
    // Handle and display error
}
```

## UI Interaction

### Selection Handling
Handle selection changes in partial methods:

```csharp
partial void OnSelectedWorkerChanged(WorkerViewModel? value)
{
    if (value != null)
    {
        // Update dependent properties
        UpdateAddressFromWorker(value);
    }
}
```

### Collection Management
Manage collections with proper sorting and filtering:

```csharp
private int FindInsertionIndex(int currentIndex, string toolName)
{
    // Binary search logic for maintaining sorted order
}
```
