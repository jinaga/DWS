using DWS.Model;

namespace DWS.Console.Areas.Tasks;


public partial class NewTaskViewModel : ObservableObject
{
    private readonly JinagaClient jinagaClient;
    private readonly Supplier supplier;

    public ObservableCollection<YardViewModel> Yards { get; } = [];
    public ObservableCollection<ToolViewModel> ToolCatalog { get; } = [];
    public ObservableCollection<TaskToolViewModel> Tools { get; } = [];

    private IObserver? yardObserver;
    private IObserver? toolObserver;

    [ObservableProperty]
    private string clientName = string.Empty;

    [ObservableProperty]
    private string yardName = string.Empty;

    [ObservableProperty]
    private YardViewModel? selectedYard;

    public NewTaskViewModel(JinagaClient jinagaClient, Supplier supplier)
    {
        this.jinagaClient = jinagaClient;
        this.supplier = supplier;
    }

    public void Load()
    {
        if (yardObserver != null)
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

        yardObserver = jinagaClient.Watch(yardsInSupplier, supplier, yardProjection =>
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

        var toolsInSupplier = Given<Supplier>.Match((supplier, facts) =>
          from tool in facts.OfType<Tool>()
          where tool.supplier == supplier && !tool.IsDeleted
          select new
          {
              tool = tool,
              toolNames = facts.Observable(tool.Names.Select(name => name.value))
          }
        );

        toolObserver = jinagaClient.Watch(toolsInSupplier, supplier, toolProjection =>
        {
            ToolViewModel tool = new ToolViewModel(toolProjection.tool);
            ToolCatalog.Insert(0, tool);

            toolProjection.toolNames.OnAdded(name =>
            {
                tool.Name = name;
                int currentIndex = ToolCatalog.IndexOf(tool);
                int newIndex = FindInsertionIndex(tool.Name);
                if (currentIndex != newIndex)
                {
                    ToolCatalog.Move(currentIndex, newIndex);
                }
            });

            return () => ToolCatalog.Remove(tool);
        });
    }

    private int FindInsertionIndex(string toolName)
    {
        int low = 0;
        int high = ToolCatalog.Count - 1;

        while (low <= high)
        {
            int mid = (low + high) / 2;
            int comparison = string.Compare(toolName, ToolCatalog[mid].Name, StringComparison.OrdinalIgnoreCase);

            if (comparison == 0)
            {
                return mid;
            }
            else if (comparison < 0)
            {
                high = mid - 1;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    public void Unload()
    {
        yardObserver?.Stop();
        yardObserver = null;
        Yards.Clear();

        toolObserver?.Stop();
        toolObserver = null;
        ToolCatalog.Clear();
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

        // Add the tools
        foreach (var taskToolViewModel in Tools)
        {
            if (taskToolViewModel.Tool != null)
            {
                await jinagaClient.Fact(new TaskToolLookup(task, taskToolViewModel.Tool, DateTime.UtcNow));
            }
            else
            {
                await jinagaClient.Fact(new TaskToolOnTheFly(task, taskToolViewModel.Name, DateTime.UtcNow));
            }
        }
    }
}
