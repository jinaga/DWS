using DWS.Model;

namespace DWS.Console.ViewModels.TaskOverview.NewTask;


public partial class NewTaskViewModel : ObservableObject
{
    private readonly JinagaClient jinagaClient;
    private readonly Supplier supplier;
    private readonly Func<Yard, YardViewModel> yardFactory;
    private readonly Func<Tool, ToolViewModel> toolFactory;
    private readonly Func<Worker, WorkerViewModel> workerFactory;

    public ObservableCollection<YardViewModel> Yards { get; } = [];
    public ObservableCollection<ToolViewModel> ToolCatalog { get; } = [];
    public ObservableCollection<TaskToolViewModel> Tools { get; } = [];
    public ObservableCollection<WorkerViewModel> Workers { get; } = []; 

    private IObserver? yardObserver;
    private IObserver? toolObserver;
    private IObserver? workerObserver;

    [ObservableProperty]
    private string clientName = string.Empty;

    [ObservableProperty]
    private string yardName = string.Empty;

    [ObservableProperty]
    private WorkerViewModel? selectedWorker;    

    [ObservableProperty]
    private YardViewModel? selectedYard;

    public NewTaskViewModel(
        JinagaClient jinagaClient, 
        Supplier supplier,
        Func<Yard, YardViewModel> yardFactory,
        Func<Tool, ToolViewModel> toolFactory,
        Func<Worker, WorkerViewModel> workerFactory)
    {
        this.jinagaClient = jinagaClient;
        this.supplier = supplier;
        this.yardFactory = yardFactory;
        this.toolFactory = toolFactory;
        this.workerFactory = workerFactory;
    }

    public void Load()
    {
        if (yardObserver != null)
        {
            return;
        }

        LoadYards();
        LoadTools();
        LoadWorkers();
    }

   
    public Task Ready()
    {
        return Task.WhenAll(
            yardObserver?.Loaded ?? Task.CompletedTask,
            toolObserver?.Loaded ?? Task.CompletedTask,
            workerObserver?.Loaded ?? Task.CompletedTask
        );
    }

    public void Unload()
    {
        UnloadYards();
        UnloadTools();
        UnloadWorkers();
    }


    private void LoadYards()
    {
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
            YardViewModel yard = yardFactory(yardProjection.yard);
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

    private void LoadTools()
    {
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
            ToolViewModel tool = toolFactory(toolProjection.tool);
            ToolCatalog.Insert(0, tool);

            toolProjection.toolNames.OnAdded(name =>
            {
                tool.Name = name;
                int currentIndex = ToolCatalog.IndexOf(tool);
                int newIndex = FindInsertionIndex(currentIndex, tool.Name);
                if (currentIndex != newIndex)
                {
                    ToolCatalog.Move(currentIndex, newIndex);
                }
            });

            return () => ToolCatalog.Remove(tool);
        });
    }

    private void LoadWorkers()
    {
       var workersInSupplier = Given<Supplier>.Match((supplier, facts) =>
         from worker in facts.OfType<Worker>()
         where worker.supplier == supplier && !worker.IsRevoked
         select new
         {
             worker,
             workerNames = facts.Observable(worker.Names.Select(name => name.value))
         }
       );


        workerObserver = jinagaClient.Watch(workersInSupplier, supplier, workerProjection =>
        {
            WorkerViewModel worker = workerFactory(workerProjection.worker);
            Workers.Add(worker);

            workerProjection.workerNames.OnAdded(name =>
            {
                worker.Name = name;
            });

            return () => Workers.Remove(worker);
        });

    }



    private void UnloadYards()
    {
        yardObserver?.Stop();
        yardObserver = null;
        Yards.Clear();
    }

    private void UnloadTools()
    {
        toolObserver?.Stop();
        toolObserver = null;
        ToolCatalog.Clear();
    }


    private void UnloadWorkers()
    {
        workerObserver?.Stop();
        workerObserver = null;
        Workers.Clear();
    }

    private int FindInsertionIndex(int currentIndex, string toolName)
    {
        // Determine which side of the current index the tool belongs on.
        if (currentIndex > 0 && string.Compare(ToolCatalog[currentIndex - 1].Name, toolName, StringComparison.OrdinalIgnoreCase) > 0)
        {
            // The tool belongs on the left side.
            return FindInsertionIndexLeft(currentIndex, toolName);
        }
        else if (currentIndex < ToolCatalog.Count - 1 && string.Compare(ToolCatalog[currentIndex + 1].Name, toolName, StringComparison.OrdinalIgnoreCase) < 0)
        {
            // The tool belongs on the right side.
            return FindInsertionIndexRight(currentIndex, toolName);
        }
        else
        {
            // The tool belongs in the current position.
            return currentIndex;
        }
    }

    private int FindInsertionIndexLeft(int currentIndex, string toolName)
    {
        int low = 0;
        int high = currentIndex - 1;

        while (low < high)
        {
            int mid = (low + high) / 2;
            int comparison = string.Compare(ToolCatalog[mid].Name, toolName, StringComparison.OrdinalIgnoreCase);

            if (comparison < 0)
            {
                low = mid + 1;
            }
            else
            {
                high = mid;
            }
        }

        return low;
    }

    private int FindInsertionIndexRight(int currentIndex, string toolName)
    {
        int low = currentIndex + 1;
        int high = ToolCatalog.Count - 1;

        while (low < high)
        {
            int mid = (low + high + 1) / 2;
            int comparison = string.Compare(ToolCatalog[mid].Name, toolName, StringComparison.OrdinalIgnoreCase);

            if (comparison < 0)
            {
                low = mid;
            }
            else
            {
                high = mid - 1;
            }
        }

        return high;
    }

    partial void OnSelectedYardChanged(YardViewModel? value)
    {
        YardName = value?.YardName ?? string.Empty;
    }

    public async Task Save()
    {
        Yard? selectedYard = SelectedYard?.Yard ?? null; 

        if (selectedYard == null)
        {
            var client = await jinagaClient.Fact(new Client(supplier, Guid.NewGuid()));
            await jinagaClient.Fact(new ClientName(client, ClientName, []));
            var yard = await jinagaClient.Fact(new Yard(client, Guid.NewGuid()));
            await jinagaClient.Fact(new YardName(yard, YardName, []));
            selectedYard = yard;

        }

        // Create the task
        var task = await jinagaClient.Fact(new DWSTask(selectedYard, Guid.NewGuid()));

        // Set the properties of the task
        await jinagaClient.Fact(new TaskClientName(task, ClientName, []));
        await jinagaClient.Fact(new TaskYardName(task, YardName, []));
        if (SelectedWorker != null)
        {
            await jinagaClient.Fact(new TaskWorker(task, SelectedWorker.Worker, []));
        }
        

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
