using DWS.Console.ViewModels.TaskOverview.NewTask;
using DWS.Model;

namespace DWS.Console.Tests.Areas.Tasks;

public class NewTaskViewModelTests : AutofacTestBase
{
    [Fact]
    public async Task WhenNoTools_ToolListIsEmpty()
    {
        // Arrange
        var supplier = await CreateSupplier();
        
        // Create a scope to resolve the view model
        using var scope = Container.BeginLifetimeScope(builder =>
        {
            // Register the supplier instance for this test
            builder.RegisterInstance(supplier).AsSelf();
        });
        
        // Resolve the view model from the container
        var viewModel = scope.Resolve<NewTaskViewModel>();
        
        // Act
        viewModel.Load();
        await viewModel.Ready();
        
        // Assert
        viewModel.ToolCatalog.Should().BeEmpty();
    }

    [Fact]
    public async Task WhenToolsExist_ToolListIsPopulated()
    {
        // Arrange
        var supplier = await CreateSupplier();
        var tool1 = await CreateTool(supplier);
        var tool2 = await CreateTool(supplier);
        
        // Create a scope to resolve the view model
        using var scope = Container.BeginLifetimeScope(builder =>
        {
            // Register the supplier instance for this test
            builder.RegisterInstance(supplier).AsSelf();
        });
        
        // Resolve the view model from the container
        var viewModel = scope.Resolve<NewTaskViewModel>();
        
        // Act
        viewModel.Load();
        await viewModel.Ready();
        
        // Assert
        viewModel.ToolCatalog.Should().HaveCount(2);
        viewModel.ToolCatalog.Should().Contain(t => t.Tool.toolGuid == tool1.toolGuid);
        viewModel.ToolCatalog.Should().Contain(t => t.Tool.toolGuid == tool2.toolGuid);
    }

    [Fact]
    public async Task WhenYardsHaveNames_YardListHasNames()
    {
        // Arrange
        var supplier = await CreateSupplier();
        var client1 = await CreateClient(supplier);
        var yard1 = await CreateYard(client1);
        await JinagaClient.Fact(new YardName(yard1, "Yard 1", []));
        
        var client2 = await CreateClient(supplier);
        var yard2 = await CreateYard(client2);
        await JinagaClient.Fact(new YardName(yard2, "Yard 2", []));
        
        // Create a scope to resolve the view model
        using var scope = Container.BeginLifetimeScope(builder =>
        {
            // Register the supplier instance for this test
            builder.RegisterInstance(supplier).AsSelf();
        });
        
        // Resolve the view model from the container
        var viewModel = scope.Resolve<NewTaskViewModel>();
        
        // Act
        viewModel.Load();
        await viewModel.Ready();
        
        // Assert
        viewModel.Yards.Should().HaveCount(2);
        viewModel.Yards.Should().Contain(y => y.Yard.yardGuid == yard1.yardGuid);
        viewModel.Yards.Should().Contain(y => y.Yard.yardGuid == yard2.yardGuid);
    }

    [Fact]
    public async Task WhenAddingToolByName_ToolIsAddedToList()
    {
        // Arrange
        var supplier = await CreateSupplier();
        
        // Create a scope to resolve the view model
        using var scope = Container.BeginLifetimeScope(builder =>
        {
            // Register the supplier instance for this test
            builder.RegisterInstance(supplier).AsSelf();
        });
        
        // Resolve the view model from the container
        var viewModel = scope.Resolve<NewTaskViewModel>();
        viewModel.Load();
        await viewModel.Ready();
        
        // Act
        viewModel.AddToolByName("Test Tool");
        
        // Assert
        viewModel.Tools.Should().HaveCount(1);
        viewModel.Tools[0].Name.Should().Be("Test Tool");
        viewModel.Tools[0].Tool.Should().BeNull(); // Tool is null for tools added by name
    }
    
    [Fact]
    public async Task WhenAddingToolByReference_ToolIsAddedToList()
    {
        // Arrange
        var supplier = await CreateSupplier();
        var tool = await CreateTool(supplier);
        await JinagaClient.Fact(new ToolName(tool, "Existing Tool", []));
        
        // Create a scope to resolve the view model
        using var scope = Container.BeginLifetimeScope(builder =>
        {
            // Register the supplier instance for this test
            builder.RegisterInstance(supplier).AsSelf();
        });
        
        // Resolve the view model from the container
        var viewModel = scope.Resolve<NewTaskViewModel>();
        viewModel.Load();
        await viewModel.Ready();
        
        // Act
        viewModel.AddTool(tool);
        
        // Assert
        viewModel.Tools.Should().HaveCount(1);
        viewModel.Tools[0].Tool.Should().NotBeNull();
        viewModel.Tools[0].Tool!.toolGuid.Should().Be(tool.toolGuid);
    }
}
