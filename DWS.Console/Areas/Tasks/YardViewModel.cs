using DWS.Model;

namespace DWS.Console.Areas.Tasks;

public partial class YardViewModel : ObservableObject
{
    public YardViewModel(Yard yard)
    {
        Yard = yard;
    }

    [ObservableProperty]
    private string clientName = string.Empty;

    [ObservableProperty]
    private string yardName = string.Empty;

    [ObservableProperty]
    private string street = string.Empty;

    [ObservableProperty]
    private string number = string.Empty;

    [ObservableProperty]
    private string postalCode = string.Empty;

    [ObservableProperty]
    private string city = string.Empty;

    [ObservableProperty]
    private string country = string.Empty;

    public Yard Yard { get; }

    public override string ToString() => ClientName;
}