using DWS.Model;

namespace DWS.Console.Tests
{
    public abstract class AutofacTestBase : IDisposable
    {
        protected IContainer Container { get; }
        protected JinagaClient JinagaClient { get; }
        
        protected AutofacTestBase()
        {
            // Build the container with the test module
            var builder = new ContainerBuilder();
            builder.RegisterModule<TestModule>();
            Container = builder.Build();
            
            // Get the Jinaga client from the container
            JinagaClient = Container.Resolve<JinagaClient>();
        }
        
        // Helper method to create a supplier for testing
        protected async Task<Supplier> CreateSupplier()
        {
            return await JinagaClient.Fact(new Supplier(new User("--- TEST USER ---"), Guid.NewGuid()));
        }
        
        // Helper method to create a tool for testing
        protected async Task<Tool> CreateTool(Supplier supplier)
        {
            return await JinagaClient.Fact(new Tool(supplier, Guid.NewGuid()));
        }
        
        // Helper method to create a worker for testing
        protected async Task<Worker> CreateWorker(Supplier supplier)
        {
            return await JinagaClient.Fact(new Worker(supplier, new User($"PK_Worker_{Guid.NewGuid()}"), DateTime.UtcNow));
        }
        
        // Helper method to create a client for testing
        protected async Task<Client> CreateClient(Supplier supplier)
        {
            return await JinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
        }
        
        // Helper method to create a yard for testing
        protected async Task<Yard> CreateYard(Client client)
        {
            return await JinagaClient.Fact(new Yard(client, Guid.NewGuid()));
        }
        
        public void Dispose()
        {
            Container.Dispose();
        }
    }
}