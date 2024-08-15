namespace DWS.Console;

static class JinagaConfig
{
    public static JinagaClient Client { get; } = CreateJinagaClient();

    private static JinagaClient CreateJinagaClient()
    {
        return JinagaClient.Create();
    }
}
