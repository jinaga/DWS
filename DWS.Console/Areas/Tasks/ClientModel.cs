namespace DWS.Console.Areas.Tasks;

public class ClientModel
{
    private Observable<string> name = new();

    public string Name
    {
        get => name.Value;
        set => name.Value = value;
    }
}
