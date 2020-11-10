using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPM.Web.Models.ViewModels
{
    public class ItemView
    {
        public Item Item { get; set; }
        public List<Task> Tasks { get; set; }
    }
}
