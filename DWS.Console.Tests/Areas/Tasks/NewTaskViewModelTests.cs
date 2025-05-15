using DWS.Console.Tests.TestInfrastructure;

namespace DWS.Console.Tests.Areas.Tasks;

public class NewTaskViewModelTests : ViewModelTestBase
{
    [Fact]
    public async Task WhenNoTools_ToolListIsEmpty()
    {
        // Given
        var supplier = await GivenSupplier();
        var viewModel = GivenNewTaskViewModel(supplier);
        
        // When
        await WhenViewModelIsLoaded(viewModel);
        
        // Then
        viewModel.ToolCatalog.Should().BeEmpty();
    }

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

    [Fact]
    public async Task WhenYardsHaveNames_YardListHasNames()
    {
        // Given
        var supplier = await GivenSupplier();
        var yard1 = await GivenYard(supplier, name: "Yard 1");
        var yard2 = await GivenYard(supplier, name: "Yard 2");
        var viewModel = GivenNewTaskViewModel(supplier);
        
        // When
        await WhenViewModelIsLoaded(viewModel);
        
        // Then
        viewModel.Yards.Should().HaveCount(2);
        viewModel.Yards.Should().Contain(y => y.Yard.yardGuid == yard1.yardGuid);
        viewModel.Yards.Should().Contain(y => y.Yard.yardGuid == yard2.yardGuid);
    }

    [Fact]
    public async Task WhenToolsHaveNames_ToolsAreSortedAlphabetically()
    {
        // Given
        var supplier = await GivenSupplier();
        await GivenTool(supplier, name: "Wrench");
        await GivenTool(supplier, name: "Hammer");
        await GivenTool(supplier, name: "Screwdriver");
        var viewModel = GivenNewTaskViewModel(supplier);
        
        // When
        await WhenViewModelIsLoaded(viewModel);
        
        // Then
        ThenToolsAreInOrder(viewModel, "Hammer", "Screwdriver", "Wrench");
    }

    [Fact]
    public async Task WhenToolNamesHaveDifferentCase_SortingIsCaseInsensitive()
    {
        // Given
        var supplier = await GivenSupplier();
        var tool1 = await GivenTool(supplier, name: "wrench");
        var tool2 = await GivenTool(supplier, name: "HAMMER");
        var tool3 = await GivenTool(supplier, name: "Screwdriver");
        var viewModel = GivenNewTaskViewModel(supplier);
        
        // When
        await WhenViewModelIsLoaded(viewModel);
        
        // Then
        ThenToolsAreInOrder(viewModel, "HAMMER", "Screwdriver", "wrench");
    }

    [Fact]
    public async Task WhenToolNameChanges_ToolPositionIsUpdated()
    {
        // Given
        var supplier = await GivenSupplier();
        var tool1 = await GivenTool(supplier, name: "Wrench");
        var tool2 = await GivenTool(supplier, name: "Hammer");
        var viewModel = GivenNewTaskViewModel(supplier);
        
        // When
        await WhenViewModelIsLoaded(viewModel);
        
        // Then
        ThenToolsAreInOrder(viewModel, "Hammer", "Wrench");
        
        // When
        await WhenToolNameIsChanged(tool1, "Adjustable Wrench");
        
        // Then
        ThenToolsAreInOrder(viewModel, "Adjustable Wrench", "Hammer");
    }

    [Fact]
    public async Task WhenToolNameChangesMultipleTimes_PositionIsCorrectlyUpdated()
    {
        // Given
        var supplier = await GivenSupplier();
        var tool = await GivenTool(supplier, name: "Middle");
        var viewModel = GivenNewTaskViewModel(supplier);
        
        // When
        await WhenViewModelIsLoaded(viewModel);
        await WhenToolNameIsChanged(tool, "Zebra");
        await WhenToolNameIsChanged(tool, "Alpha");
        
        // Then
        viewModel.ToolCatalog[0].Name.Should().Be("Alpha");
    }

    [Fact]
    public async Task WhenMultipleToolsChangeNamesConcurrently_OrderingIsCorrect()
    {
        // Given
        var supplier = await GivenSupplier();
        var tool1 = await GivenTool(supplier, name: "A");
        var tool2 = await GivenTool(supplier, name: "B");
        var tool3 = await GivenTool(supplier, name: "C");
        var viewModel = GivenNewTaskViewModel(supplier);
        
        // When
        await WhenViewModelIsLoaded(viewModel);
        
        // When
        await WhenMultipleToolNamesAreChangedConcurrently(
            (tool1, "Z"),
            (tool2, "Y"),
            (tool3, "X")
        );
        
        // Then
        ThenToolsAreInOrder(viewModel, "X", "Y", "Z");
    }

    [Fact]
    public async Task WhenToolMovesToBeginningOfList_PositionIsCorrect()
    {
        // Given
        var supplier = await GivenSupplier();
        var tool1 = await GivenTool(supplier, name: "M");
        var tool2 = await GivenTool(supplier, name: "N");
        var targetTool = await GivenTool(supplier, name: "O");
        var viewModel = GivenNewTaskViewModel(supplier);
        
        // When
        await WhenViewModelIsLoaded(viewModel);
        
        // When
        await WhenToolNameIsChanged(targetTool, "A");
        
        // Then
        ThenToolIsAtPosition(viewModel, targetTool, 0);
    }

    [Fact]
    public async Task WhenToolMovesToEndOfList_PositionIsCorrect()
    {
        // Given
        var supplier = await GivenSupplier();
        var tool1 = await GivenTool(supplier, name: "M");
        var tool2 = await GivenTool(supplier, name: "N");
        var targetTool = await GivenTool(supplier, name: "O");
        var viewModel = GivenNewTaskViewModel(supplier);
        
        // When
        await WhenViewModelIsLoaded(viewModel);
        
        // When
        await WhenToolNameIsChanged(targetTool, "Z");
        
        // Then
        ThenToolIsAtPosition(viewModel, targetTool, viewModel.ToolCatalog.Count - 1);
    }
}
