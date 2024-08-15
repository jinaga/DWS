using DWS.Model;

namespace DWS.Console.Areas.Tasks;

public class NewTaskViewModel(JinagaClient jinagaClient)
{
    private ObservableList<ClientModel> clients = new ObservableList<ClientModel>();

    public IEnumerable<ClientViewModel> Clients =>
        clients.Select(client => new ClientViewModel(client));

    public void Load()
    {
        var yardsInSupplier = Given<Supplier>.Match((supplier, facts) =>
          from client in facts.OfType<Client>()
          where client.supplier == supplier
          from yard in facts.OfType<Yard>()
          where yard.client == client && !yard.IsDeleted
          select new
          {
              clientNames = facts.Observable(client.Names.Select(name => name.value)),
              yardNames = yard.Names.Select(name => name.value),
              yardAddresses = yard.Addresses.Select(address => new
              {
                  street = address.street,
                  number = address.number,
                  postalCode = address.postalCode,
                  city = address.place,
                  country = address.country
              })
          }
        );

        Supplier supplier = null!;
        jinagaClient.Watch(yardsInSupplier, supplier, yardProjection =>
        {
            ClientModel client = new ClientModel();
            clients.Add(client);

            yardProjection.clientNames.OnAdded(name =>
            {
                client.Name = name;
            });
        });
    }
}
