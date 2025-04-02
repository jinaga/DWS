using DWS.Console.ViewModels.Tasks;
using DWS.Model;

namespace DWS.Console.Tests.Areas.Tasks;

public class NewTaskViewModelTests
{
    [Fact]
    public async Task WhenNoTools_ToolListIsEmpty()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        viewModel.ToolCatalog.Should().BeEmpty();
    }

    [Fact]
    public async Task WhenToolsExist_ToolListIsPopulated()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

        var tool1 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool2 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        viewModel.ToolCatalog.Should().HaveCount(2);
        viewModel.ToolCatalog.Should().Contain(t => t.Tool.toolGuid == tool1.toolGuid);
        viewModel.ToolCatalog.Should().Contain(t => t.Tool.toolGuid == tool2.toolGuid);
    }

    [Fact]
    public async Task WhenYardsHaveNames_YardListHasNames()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

        var yard1 = await jinagaClient.Fact(new Yard(new Client(supplier, Guid.NewGuid()), Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yard1, "Yard 1", []));
        var yard2 = await jinagaClient.Fact(new Yard(new Client(supplier, Guid.NewGuid()), Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yard2, "Yard 2", []));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        viewModel.Yards.Should().HaveCount(2);
        viewModel.Yards.Should().Contain(y => y.Yard.yardGuid == yard1.yardGuid);
        viewModel.Yards.Should().Contain(y => y.Yard.yardGuid == yard2.yardGuid);
    }

    [Fact]
    public async Task WhenToolsHaveNames_ToolsAreSortedAlphabetically()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

        var tool1 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool2 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool3 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new ToolName(tool1, "Wrench", []));
        await jinagaClient.Fact(new ToolName(tool2, "Hammer", []));
        await jinagaClient.Fact(new ToolName(tool3, "Screwdriver", []));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        viewModel.ToolCatalog.Select(t => t.Name).Should().Equal(
            "Hammer",
            "Screwdriver",
            "Wrench");
    }

    [Fact]
    public async Task WhenToolNamesHaveDifferentCase_SortingIsCaseInsensitive()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

        var tool1 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool2 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool3 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        await jinagaClient.Fact(new ToolName(tool1, "wrench", []));
        await jinagaClient.Fact(new ToolName(tool2, "HAMMER", []));
        await jinagaClient.Fact(new ToolName(tool3, "Screwdriver", []));

        viewModel.ToolCatalog.Select(t => t.Name).Should().Equal(
            "HAMMER",
            "Screwdriver",
            "wrench");
    }

    [Fact]
    public async Task WhenToolNameChanges_ToolPositionIsUpdated()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

        var tool1 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool2 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        await jinagaClient.Fact(new ToolName(tool1, "Wrench", []));
        await jinagaClient.Fact(new ToolName(tool2, "Hammer", []));

        // Assert initial order
        viewModel.ToolCatalog.Select(t => t.Name).Should().Equal(
            "Hammer",
            "Wrench");

        // Change name of first tool
        await jinagaClient.Fact(new ToolName(tool1, "Adjustable Wrench", []));

        // Assert new order
        viewModel.ToolCatalog.Select(t => t.Name).Should().Equal(
            "Adjustable Wrench",
            "Hammer");
    }

    [Fact]
    public async Task WhenToolNameChangesMultipleTimes_PositionIsCorrectlyUpdated()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));
        var tool = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        await jinagaClient.Fact(new ToolName(tool, "Middle", []));
        await jinagaClient.Fact(new ToolName(tool, "Zebra", []));
        await jinagaClient.Fact(new ToolName(tool, "Alpha", []));

        // Should end up at the beginning of the list
        viewModel.ToolCatalog[0].Name.Should().Be("Alpha");
    }

    [Fact]
    public async Task WhenMultipleToolsChangeNamesConcurrently_OrderingIsCorrect()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));
        var tool1 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool2 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool3 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        // Create initial ordering
        await jinagaClient.Fact(new ToolName(tool1, "A", []));
        await jinagaClient.Fact(new ToolName(tool2, "B", []));
        await jinagaClient.Fact(new ToolName(tool3, "C", []));

        // Change all names "simultaneously"
        await Task.WhenAll(
            jinagaClient.Fact(new ToolName(tool1, "Z", [])),
            jinagaClient.Fact(new ToolName(tool2, "Y", [])),
            jinagaClient.Fact(new ToolName(tool3, "X", []))
        );

        viewModel.ToolCatalog.Select(t => t.Name)
            .Should().Equal("X", "Y", "Z");
    }

    [Fact]
    public async Task WhenToolMovesToBeginningOfList_PositionIsCorrect()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

        // Create tools with names in middle of alphabet
        var tool1 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool2 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var targetTool = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        await jinagaClient.Fact(new ToolName(tool1, "M", []));
        await jinagaClient.Fact(new ToolName(tool2, "N", []));
        await jinagaClient.Fact(new ToolName(targetTool, "O", []));

        // Move target tool to beginning
        await jinagaClient.Fact(new ToolName(targetTool, "A", []));

        viewModel.ToolCatalog[0].Tool.toolGuid.Should().Be(targetTool.toolGuid);
    }

    [Fact]
    public async Task WhenToolMovesToEndOfList_PositionIsCorrect()
    {
        var jinagaClient = JinagaClient.Create();
        var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

        // Create tools with names in middle of alphabet
        var tool1 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var tool2 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        var targetTool = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));

        var viewModel = new NewTaskViewModel(jinagaClient, supplier);
        viewModel.Load();
        await viewModel.Ready();

        await jinagaClient.Fact(new ToolName(tool1, "M", []));
        await jinagaClient.Fact(new ToolName(tool2, "N", []));
        await jinagaClient.Fact(new ToolName(targetTool, "O", []));

        // Move target tool to end
        await jinagaClient.Fact(new ToolName(targetTool, "Z", []));

        viewModel.ToolCatalog[^1].Tool.toolGuid.Should().Be(targetTool.toolGuid);
    }
}
