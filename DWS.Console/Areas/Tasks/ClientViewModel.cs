namespace DWS.Console.Areas.Tasks;

public class ClientViewModel(ClientModel client)
{
    public string Name => client.Name;
}
