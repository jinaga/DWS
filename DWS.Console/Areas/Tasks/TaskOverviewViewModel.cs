using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWS.Model;

namespace DWS.Console.Areas.Tasks
{
    public partial class TaskOverviewViewModel: ObservableObject
    {

        private readonly JinagaClient jinagaClient;
        private readonly Supplier supplier;


        public ObservableCollection<TaskViewModel> Tasks { get; } = [];



        public TaskOverviewViewModel(JinagaClient jinagaClient, Supplier supplier)
        {
            this.jinagaClient = jinagaClient; 
            this.supplier = supplier;

        }
        public void Load()
        {
          
        }
        public void Unload()
        {
          
        }   
    }
}
