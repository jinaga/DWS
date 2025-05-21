using Autofac;
using DWS.Console.ViewModels.TaskOverview.NewTask;
using DWS.Model;

namespace DWS.Console.Tests.TestInfrastructure;

public abstract class ViewModelTestBase
{
    protected IContainer Container { get; }
    protected JinagaClient JinagaClient { get; }
    
    protected ViewModelTestBase()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<TestModule>();
        Container = builder.Build();
        
        JinagaClient = Container.Resolve<JinagaClient>();
    }
    
    // Gherkin-style methods
    protected async Task<Supplier> GivenSupplier()
    {
        return await JinagaClient.Fact(new Supplier(new User("--- SUPPLIER CREATOR ---"), Guid.NewGuid()));
    }
    
    protected NewTaskViewModel GivenNewTaskViewModel(Supplier supplier)
    {
        return Container.Resolve<Func<Supplier, NewTaskViewModel>>()(supplier);
    }
    
    protected async Task WhenViewModelIsLoaded(NewTaskViewModel viewModel)
    {
        viewModel.Load();
        await viewModel.Ready();
    }
    
    protected async Task<Tool> GivenTool(Supplier supplier, string? name = null)
    {
        var tool = await JinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        if (name != null)
        {
            await JinagaClient.Fact(new ToolName(tool, name, []));
        }
        return tool;
    }
    
    protected async Task<Yard> GivenYard(Supplier supplier, string? name = null)
    {
        var client = await JinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
        var yard = await JinagaClient.Fact(new Yard(client, Guid.NewGuid()));
        if (name != null)
        {
            await JinagaClient.Fact(new YardName(yard, name, []));
        }
        return yard;
    }
    
    protected async Task<Client> GivenClient(Supplier supplier)
    {
        return await JinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
    }
    
    protected async Task<Worker> GivenWorker(Supplier supplier)
    {
        var user = new User("--- WORKER USER ---");
        return await JinagaClient.Fact(new Worker(supplier, user, DateTime.UtcNow));
    }
    
    
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
}
