using DWS.Console.Areas.Tasks;
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
    }
}
