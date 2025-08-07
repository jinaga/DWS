
using CommunityToolkit.Mvvm.Input;
using Jinaga;
using System.Diagnostics;
using System.Windows.Input;

namespace DWS.Console.ViewModels.TaskOverview
{

    public partial class TaskViewModel : ObservableObject
    {

        private readonly JinagaClient jinagaClient;      

        public TaskViewModel(JinagaClient jinagaClient, DWSTask task)
        {
            this.jinagaClient = jinagaClient;
            Task = task;
        }

        public DWSTask Task { get; }


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

      
        [RelayCommand(AllowConcurrentExecutions=true,CanExecute = nameof(CanDeleteTask))   ]
        private async Task DeleteTask()
        {
            Debug.Print("DeleteTaskCommand");
            await jinagaClient.Fact(new TaskDelete(Task, DateTime.Now));
        }

      
        private bool CanDeleteTask()
        {
            return true;
            //deleteTaskCommand?.NotifyCanExecuteChanged();
        }




    }
}
