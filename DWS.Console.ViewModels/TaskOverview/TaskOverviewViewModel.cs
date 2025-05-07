using System.Diagnostics;

namespace DWS.Console.ViewModels.TaskOverview
{
    public partial class TaskOverviewViewModel: ObservableObject
    {
        private readonly JinagaClient jinagaClient;
        private readonly Supplier supplier;
        private readonly Func<DWSTask, TaskViewModel> taskViewModelFactory;

        public ObservableCollection<TaskViewModel> Tasks { get; } = [];

        private IObserver? taskObserver;

        [ObservableProperty]
        private TaskViewModel? selectedTask;

        public TaskOverviewViewModel(
            JinagaClient jinagaClient,
            Supplier supplier,
            Func<DWSTask, TaskViewModel> taskViewModelFactory)
        {
            this.jinagaClient = jinagaClient;
            this.supplier = supplier;
            this.taskViewModelFactory = taskViewModelFactory;
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
            // Simplified query to find all tasks regardless of deletion status
            var tasksInSupplier = Given<Supplier>.Match((supplier, facts) =>
                from client in facts.OfType<Client>()
                where client.supplier == supplier
                from yard in facts.OfType<Yard>()
                where yard.client == client
                from task in facts.OfType<DWSTask>()
                where task.yard == yard
                select new
                {
                    DWSTask = task,
                    clientNames = facts.Observable(task.ClientNames.Select(name => name.value)),
                    yardNames = facts.Observable(task.YardNames.Select(name => name.value))
                }
            );
    
            taskObserver = jinagaClient.Watch(tasksInSupplier, supplier, taskProjection =>
            {
                // Use the factory to create the TaskViewModel
                var task = taskViewModelFactory(taskProjection.DWSTask);
                Tasks.Add(task);
                
                // Debug log
                Debug.WriteLine($"Task added: {taskProjection.DWSTask.taskGuid}");
    
                taskProjection.clientNames.OnAdded(name =>
                {
                    task.ClientName = name;
                    Debug.WriteLine($"Client name added: {name}");
                });
    
                taskProjection.yardNames.OnAdded(name =>
                {
                    task.YardName = name;
                    Debug.WriteLine($"Yard name added: {name}");
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
}
