using DWS.Model;

namespace DWS.Console;

static class JinagaConfig
{
    public static JinagaClient CreateJinagaClient()
    {
        JinagaClient jinagaClient = JinagaClient.Create();

        return jinagaClient;
    }

    public static async Task<Supplier> CreateSampleData(JinagaClient jinagaClient)
    {
        var supplier = await jinagaClient.SingleUse(async creator =>
        {
            var supplier = await jinagaClient.Fact(new Supplier(creator, Guid.NewGuid()));
            return supplier;
        });

        await CreateSampleTools(jinagaClient, supplier);
        await CreateSampleWorkers(jinagaClient, supplier);
        await CreateSampleClientsAndYards(jinagaClient, supplier);   

        return supplier;
    }

    private static async Task CreateSampleWorkers(JinagaClient jinagaClient, Supplier supplier)
    {
        await CreateWorker("Jan");
        await CreateWorker("Michael");
        await CreateWorker("King Gip");
        await CreateWorker("Luc");
        await CreateWorker("Karen");
        await CreateWorker("Thomas");

        async Task CreateWorker(string userName)
        {
            var user = await jinagaClient.Fact(new User($"PK_{userName}"));
            await jinagaClient.Fact(new UserName(user, userName, []));
            await jinagaClient.Fact(new Worker(supplier, user, DateTime.UtcNow));            
        }

    }

    private static async Task CreateSampleTools(JinagaClient jinagaClient, Supplier supplier)
    {
        await CreateTool("Truck 50T");
        await CreateTool("Spade");
        await CreateTool("Lawn mower");
        await CreateTool("GPS equipment");
        await CreateTool("Brochures");

        async Task CreateTool(string name)
        {
            var user = await jinagaClient.Fact(new User("PK_Jan"));
            var tool = await jinagaClient.Fact(new Tool(supplier, Guid.NewGuid(), user));
            await jinagaClient.Fact(new ToolName(tool, name, []));
        }
    }

    private static async Task CreateSampleClientsAndYards(JinagaClient jinagaClient, Supplier supplier)
    {
        var clientA = await jinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new ClientName(clientA, "Client A", []));
        
        var clientB = await jinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new ClientName(clientB, "Client B", []));

        var clientC = await jinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new ClientName(clientB, "Client C", []));

        var yardA1 = await jinagaClient.Fact(new Yard(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yardA1, "Yard A1", []));
        await jinagaClient.Fact(new YardAddress(
            yardA1,
            "Main Street",
            "1",
            "12345",
            "Anytown",
            "USA",
            []));

        var yardB1 = await jinagaClient.Fact(new Yard(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yardB1, "Yard B1", []));
        await jinagaClient.Fact(new YardAddress(
            yardB1,
            "Second Street",
            "2",
            "23456",
            "Othertown",
            "USA",
            []));

        var yardB2 = await jinagaClient.Fact(new Yard(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yardB2, "Yard B2", []));
        await jinagaClient.Fact(new YardAddress(
            yardB2,
            "Third Street",
            "3",
            "34567",
            "Another Town",
            "USA",
            []));

        var yardB3C1 = await jinagaClient.Fact(new Yard(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yardB2, "Yard B3C1", []));
        await jinagaClient.Fact(new YardAddress(
            yardB3C1,
            "Unknown Street",
            "7",
            "8645",
            "ThisTown",
            "USA",
            []));

        await jinagaClient.Fact(new YardClients(yardA1, clientA, DateTime.UtcNow));
        await jinagaClient.Fact(new YardClients(yardB1, clientB, DateTime.UtcNow));
        await jinagaClient.Fact(new YardClients(yardB2, clientB, DateTime.UtcNow));
        await jinagaClient.Fact(new YardClients(yardB3C1, clientB, DateTime.UtcNow));
        await jinagaClient.Fact(new YardClients(yardB3C1, clientC, DateTime.UtcNow));
    }

}
