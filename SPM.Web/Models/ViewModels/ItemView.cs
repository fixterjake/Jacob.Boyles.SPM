using System.Collections.Generic;

namespace SPM.Web.Models.ViewModels
{
    public class ItemView
    {
        public Item Item { get; set; }
        public List<Task> Tasks { get; set; }
    }
}