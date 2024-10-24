using DWS.Console.ViewModels.Tasks;
using DWS.Model;

namespace DWS.Console.Tests.Areas.Tasks
{
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
    }
}
