---
globs: DWS.Console.Tests/**/*.cs
description: Use this rule when writing unit tests, creating test data, implementing test infrastructure, or when you need to understand testing conventions. This rule covers test structure, assertions, async testing, and test organization patterns.
---
# Testing Patterns

## Test Structure

### Base Class
All tests inherit from `ViewModelTestBase` for common setup:

```csharp
public class NewTaskViewModelTests : ViewModelTestBase
{
    // Test methods
}
```

### Test Naming
Use Gherkin-style naming: `Given_When_Then`:

```csharp
[Fact]
public async Task WhenNoTools_ToolListIsEmpty()
{
    // Test implementation
}
```

## Test Infrastructure

### Container Setup
Use Autofac container for dependency resolution in tests:

```csharp
protected IContainer Container { get; }
protected JinagaClient JinagaClient { get; }

protected ViewModelTestBase()
{
    var builder = new ContainerBuilder();
    builder.RegisterModule<TestModule>();
    Container = builder.Build();
    
    JinagaClient = Container.Resolve<JinagaClient>();
}
```

### Factory Methods
Create helper methods for test data setup:

```csharp
protected async Task<Supplier> GivenSupplier()
{
    return await JinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));
}

protected NewTaskViewModel GivenNewTaskViewModel(Supplier supplier)
{
    return Container.Resolve<Func<Supplier, NewTaskViewModel>>()(supplier);
}
```

## Test Data Creation

### Fact Creation
Use `JinagaClient.Fact()` for creating test facts:

```csharp
protected async Task<Tool> GivenTool(Supplier supplier, string? name = null)
{
    var creator = new User("--- TOOL CREATOR ---");
    var tool = await JinagaClient.Fact(new Tool(supplier, Guid.NewGuid(), creator));
    if (name != null)
    {
        await JinagaClient.Fact(new ToolName(tool, name, []));
    }
    return tool;
}
```

### Async Setup
Use `await` for async test setup:

```csharp
protected async Task WhenViewModelIsLoaded(NewTaskViewModel viewModel)
{
    viewModel.Load();
    await viewModel.Ready();
}
```

## Test Patterns

### Arrange-Act-Assert
Follow the AAA pattern with Gherkin-style method names:

```csharp
[Fact]
public async Task WhenToolsExist_ToolListIsPopulated()
{
    // Given
    var supplier = await GivenSupplier();
    var tool1 = await GivenTool(supplier);
    var tool2 = await GivenTool(supplier);
    var viewModel = GivenNewTaskViewModel(supplier);
    
    // When
    await WhenViewModelIsLoaded(viewModel);
    
    // Then
    viewModel.ToolCatalog.Should().HaveCount(2);
    viewModel.ToolCatalog.Should().Contain(t => t.Tool.toolGuid == tool1.toolGuid);
    viewModel.ToolCatalog.Should().Contain(t => t.Tool.toolGuid == tool2.toolGuid);
}
```

### State Changes
Test state changes with helper methods:

```csharp
protected async Task WhenToolNameIsChanged(Tool tool, string newName)
{
    await JinagaClient.Fact(new ToolName(tool, newName, []));
}

protected async Task WhenMultipleToolNamesAreChangedConcurrently(
    params (Tool tool, string newName)[] toolNamePairs)
{
    var tasks = toolNamePairs.Select(pair => 
        JinagaClient.Fact(new ToolName(pair.tool, pair.newName, [])));
    await Task.WhenAll(tasks);
}
```

## Assertions

### FluentAssertions
Use FluentAssertions for readable assertions:

```csharp
protected void ThenToolsAreInOrder(
    NewTaskViewModel viewModel, params string[] expectedNames)
{
    viewModel.ToolCatalog.Select(t => t.Name).Should().Equal(expectedNames);
}

protected void ThenToolIsAtPosition(
    NewTaskViewModel viewModel, Tool tool, int position)
{
    viewModel.ToolCatalog[position].Tool.toolGuid.Should().Be(tool.toolGuid);
}
```

### Collection Assertions
Use specific collection assertion methods:

```csharp
viewModel.ToolCatalog.Should().BeEmpty();
viewModel.ToolCatalog.Should().HaveCount(2);
viewModel.ToolCatalog.Should().Contain(t => t.Tool.toolGuid == tool1.toolGuid);
```

## Test Organization

### Test Classes
Group related tests in the same test class:

```csharp
public class NewTaskViewModelTests : ViewModelTestBase
{
    // All tests related to NewTaskViewModel
}
```

### Test Methods
Use descriptive test method names that explain the scenario:

```csharp
[Fact]
public async Task WhenToolNamesHaveDifferentCase_SortingIsCaseInsensitive()
{
    // Test implementation
}
```

## Async Testing

### Async Test Methods
Use `async Task` for test methods that perform async operations:

```csharp
[Fact]
public async Task WhenViewModelIsLoaded_DataIsPopulated()
{
    // Async test implementation
}
```

### Parallel Operations
Test concurrent operations with `Task.WhenAll()`:

```csharp
[Fact]
public async Task WhenMultipleToolsChangeNamesConcurrently_OrderingIsCorrect()
{
    // Given
    var supplier = await GivenSupplier();
    var tool1 = await GivenTool(supplier, name: "Wrench");
    var tool2 = await GivenTool(supplier, name: "Hammer");
    var viewModel = GivenNewTaskViewModel(supplier);
    await WhenViewModelIsLoaded(viewModel);
    
    // When
    await WhenMultipleToolNamesAreChangedConcurrently(
        (tool1, "Screwdriver"),
        (tool2, "Drill")
    );
    
    // Then
    ThenToolsAreInOrder(viewModel, "Drill", "Screwdriver");
}
```

## Test Data Management

### Meaningful Test Data
Use descriptive names for test data:

```csharp
var creator = new User("--- TOOL CREATOR ---");
var supplier = await JinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));
```

### Test Isolation
Each test should be independent and not rely on other tests:

```csharp
[Fact]
public async Task WhenNoTools_ToolListIsEmpty()
{
    // This test should work regardless of other tests
    var supplier = await GivenSupplier();
    var viewModel = GivenNewTaskViewModel(supplier);
    
    await WhenViewModelIsLoaded(viewModel);
    
    viewModel.ToolCatalog.Should().BeEmpty();
}
```
