using DWS.Console.ViewModels.Tasks;
using DWS.Model;
using Jinaga;

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

            Assert.Empty(viewModel.ToolCatalog);
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

            Assert.Equal(2, viewModel.ToolCatalog.Count);
            Assert.Contains(viewModel.ToolCatalog, t => t.Tool.toolGuid == tool1.toolGuid);
            Assert.Contains(viewModel.ToolCatalog, t => t.Tool.toolGuid == tool2.toolGuid);
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

            Assert.Equal(2, viewModel.Yards.Count);
            Assert.Contains(viewModel.Yards, y => y.Yard.yardGuid == yard1.yardGuid);
            Assert.Contains(viewModel.Yards, y => y.Yard.yardGuid == yard2.yardGuid);
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

            Assert.Equal(3, viewModel.ToolCatalog.Count);
            Assert.Equal("Hammer", viewModel.ToolCatalog[0].Name);
            Assert.Equal("Screwdriver", viewModel.ToolCatalog[1].Name);
            Assert.Equal("Wrench", viewModel.ToolCatalog[2].Name);
        }

        [Fact]
        public async Task WhenToolNamesHaveDifferentCase_SortingIsCaseInsensitive()
        {
            // Arrange
            var jinagaClient = JinagaClient.Create();
            var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

            var tool1 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
            var tool2 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
            var tool3 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));

            var viewModel = new NewTaskViewModel(jinagaClient, supplier);
            viewModel.Load();
            await viewModel.Ready();

            // Act
            await jinagaClient.Fact(new ToolName(tool1, "wrench", []));
            await jinagaClient.Fact(new ToolName(tool2, "HAMMER", []));
            await jinagaClient.Fact(new ToolName(tool3, "Screwdriver", []));

            // Assert
            Assert.Equal(3, viewModel.ToolCatalog.Count);
            Assert.Equal("HAMMER", viewModel.ToolCatalog[0].Name);
            Assert.Equal("Screwdriver", viewModel.ToolCatalog[1].Name);
            Assert.Equal("wrench", viewModel.ToolCatalog[2].Name);
        }

        [Fact]
        public async Task WhenToolNameChanges_ToolPositionIsUpdated()
        {
            // Arrange
            var jinagaClient = JinagaClient.Create();
            var supplier = await jinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));

            var tool1 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
            var tool2 = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));

            var viewModel = new NewTaskViewModel(jinagaClient, supplier);
            viewModel.Load();
            await viewModel.Ready();

            // Act - First set of names
            await jinagaClient.Fact(new ToolName(tool1, "Wrench", []));
            await jinagaClient.Fact(new ToolName(tool2, "Hammer", []));

            // Assert initial order
            Assert.Equal("Hammer", viewModel.ToolCatalog[0].Name);
            Assert.Equal("Wrench", viewModel.ToolCatalog[1].Name);

            // Act - Change name of first tool
            await jinagaClient.Fact(new ToolName(tool1, "Adjustable Wrench", []));

            // Assert new order
            Assert.Equal("Adjustable Wrench", viewModel.ToolCatalog[0].Name);
            Assert.Equal("Hammer", viewModel.ToolCatalog[1].Name);
        }
    }
}
