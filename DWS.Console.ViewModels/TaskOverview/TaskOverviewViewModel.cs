using DWS.Model;

namespace DWS.Console.ViewModels.TaskOverview;

public partial class TaskOverviewViewModel: ObservableObject
{

    private readonly JinagaClient jinagaClient;
    private readonly Supplier supplier;
    private readonly Func<DWSTask, TaskViewModel> taskFactory;


    public ObservableCollection<TaskViewModel> Tasks { get; } = [];

    private IObserver? taskObserver;

    [ObservableProperty]
    private TaskViewModel? selectedTask;


    public TaskOverviewViewModel(JinagaClient jinagaClient, Supplier supplier, Func<DWSTask, TaskViewModel> taskFactory)
    {
        this.jinagaClient = jinagaClient;
        this.supplier = supplier;
        this.taskFactory = taskFactory;
    }


    public void Load()
    {
        if (taskObserver != null)
        {
            return;
        }
        LoadTasks();    
    }

    public void Unload()
    {
        UnloadTasks();
    }  
    

    private void LoadTasks()
    {
        var tasksInSupplier = Given<Supplier>.Match((supplier, facts) =>
            from yard in facts.OfType<Yard>()
            where yard.supplier == supplier && !yard.IsDeleted      
            from yardClients in facts.OfType<YardClients>()
            where yardClients.yard == yard && !yardClients.IsDeleted
            from client in facts.OfType<Client>()
            where client == yardClients.client && !client.IsDeleted
            from DWSTask in facts.OfType<DWSTask>()
            where DWSTask.supplierPeriod.supplier == supplier && !DWSTask.IsDeleted    
            select new
            { 
                DWSTask,
                clientNames = facts.Observable(client.Names.Select(name => name.value)),
                yardNames = facts.Observable(DWSTask.YardNames.Select(name => name.value)),
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

        taskObserver = jinagaClient.Watch(tasksInSupplier, supplier, taskProjection =>
        {
            var task = taskFactory(taskProjection.DWSTask);
            Tasks.Add(task);

            taskProjection.clientNames.OnAdded(name =>
            {
                task.ClientName = name;
            });

            taskProjection.yardNames.OnAdded(name =>
            {
                task.YardName = name;
            });

            taskProjection.yardAddresses.OnAdded(address =>
            {
                task.Street = address.street;
                task.Number = address.number;
                task.PostalCode = address.postalCode;
                task.City = address.city;
                task.Country = address.country;
            });

           
            return () => Tasks.Remove(task);
        });
    }


    private void UnloadTasks()
    {
        taskObserver?.Stop();
        taskObserver = null;
        Tasks.Clear();
    }
   
}
