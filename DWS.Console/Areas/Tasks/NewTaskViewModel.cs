using CommunityToolkit.Mvvm.Input;
using DWS.Model;
using System.Diagnostics.Metrics;
using System.Windows.Controls;
using System.Windows.Input;

namespace DWS.Console.Areas.Tasks;


public partial class NewTaskViewModel : ObservableObject
{
    private readonly JinagaClient jinagaClient;
    private readonly Supplier supplier;

    public ObservableCollection<YardViewModel> Yards { get; } = [];

    private IObserver? observer;

    [ObservableProperty]
    private string clientName = string.Empty;

    [ObservableProperty]
    private string yardName = string.Empty;

    [ObservableProperty]
    private YardViewModel? selectedYard;

    public ObservableCollection<string> Tools { get; } = [
        "Truck 50T",
        "Spade",
        "Lawn mower",
        "GPS equipment",
        "Brochures"
    ];

    public NewTaskViewModel(JinagaClient jinagaClient, Supplier supplier)
    {
        this.jinagaClient = jinagaClient;
        this.supplier = supplier;
    }

    public void Load()
    {
        if (observer != null)
        {
            return;
        }

        var yardsInSupplier = Given<Supplier>.Match((supplier, facts) =>
          from client in facts.OfType<Client>()
          where client.supplier == supplier
          from yard in facts.OfType<Yard>()
          where yard.client == client && !yard.IsDeleted
          select new
          {
              yard = yard,
              clientNames = facts.Observable(client.Names.Select(name => name.value)),
              yardNames = facts.Observable(yard.Names.Select(name => name.value)),
              yardAddresses = facts.Observable(yard.Addresses.Select(address => new
              {
                  street = address.street,
                  number = address.number,
                  postalCode = address.postalCode,
                  city = address.place,
                  country = address.country
              }))
          }
        );

        observer = jinagaClient.Watch(yardsInSupplier, supplier, yardProjection =>
        {
            YardViewModel yard = new YardViewModel(yardProjection.yard);
            Yards.Add(yard);

            yardProjection.clientNames.OnAdded(name =>
            {
                yard.ClientName = name;
            });

            yardProjection.yardNames.OnAdded(name =>
            {
                yard.YardName = name;
            });

            yardProjection.yardAddresses.OnAdded(address =>
            {
                yard.Street = address.street;
                yard.Number = address.number;
                yard.PostalCode = address.postalCode;
                yard.City = address.city;
                yard.Country = address.country;
            });

            return () => Yards.Remove(yard);
        });
    }

    public void Unload()
    {
        observer?.Stop();
        observer = null;
        Yards.Clear();
    }

    partial void OnSelectedYardChanged(YardViewModel? value)
    {
        YardName = value?.YardName ?? string.Empty;
    }

    public async Task Save()
    {
        if (SelectedYard == null)
        {
            return;
        }

        // Create the task
        var task = await jinagaClient.Fact(new DWSTask(SelectedYard.Yard, Guid.NewGuid()));

        // Set the properties of the task
        await jinagaClient.Fact(new TaskClientName(task, ClientName, []));
        await jinagaClient.Fact(new TaskYardName(task, YardName, []));
    }
}
