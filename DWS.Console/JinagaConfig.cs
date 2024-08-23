using DWS.Model;

namespace DWS.Console;

static class JinagaConfig
{
    public static JinagaClient Client { get; } = CreateJinagaClient();

    private static JinagaClient CreateJinagaClient()
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

        var clientA = await jinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new ClientName(clientA, "Client A", []));
        var yardA1 = await jinagaClient.Fact(new Yard(clientA, Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yardA1, "Yard A1", []));
        await jinagaClient.Fact(new YardAddress(
            yardA1,
            "Main Street",
            "1",
            "12345",
            "Anytown",
            "USA",
            []));

        var clientB = await jinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
        await jinagaClient.Fact(new ClientName(clientB, "Client B", []));
        var yardB1 = await jinagaClient.Fact(new Yard(clientB, Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yardB1, "Yard B1", []));
        await jinagaClient.Fact(new YardAddress(
            yardB1,
            "Second Street",
            "2",
            "23456",
            "Othertown",
            "USA",
            []));
        var yardB2 = await jinagaClient.Fact(new Yard(clientB, Guid.NewGuid()));
        await jinagaClient.Fact(new YardName(yardB2, "Yard B2", []));
        await jinagaClient.Fact(new YardAddress(
            yardB2,
            "Third Street",
            "3",
            "34567",
            "Another Town",
            "USA",
            []));

        return supplier;
    }
}
