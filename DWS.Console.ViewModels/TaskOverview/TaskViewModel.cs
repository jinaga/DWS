
namespace DWS.Console.Areas.Tasks
{

    public partial class TaskViewModel : ObservableObject
    {

        public TaskViewModel(DWSTask task)
        {
            Task = task;
        }

        public DWSTask Task { get; }


        [ObservableProperty]
        private string clientName = string.Empty;

        [ObservableProperty]
        private string yardName = string.Empty;
    }
}
