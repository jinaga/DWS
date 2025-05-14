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
            from client in facts.OfType<Client>()
            where client.supplier == supplier && !client.IsDeleted  
            from yard in facts.OfType<Yard>()
            where yard.client == client && !yard.IsDeleted      
            from DWSTask in facts.OfType<DWSTask>()
            where DWSTask.yard == yard && !DWSTask.IsDeleted    
            select new
            { 
                DWSTask,
                clientNames = facts.Observable(DWSTask.ClientNames.Select(name => name.value)),
                yardNames = facts.Observable(DWSTask.YardNames.Select(name => name.value))
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
