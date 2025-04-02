using DWS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWS.Console.Areas.Tasks
{

    public partial class TaskViewModel : ObservableObject
    {
        [ObservableProperty]
        private string clientName = string.Empty;

        [ObservableProperty]
        private string yardName = string.Empty;
    }
}
