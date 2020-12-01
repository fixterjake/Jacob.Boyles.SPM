using System.Collections.Generic;

namespace SPM.Web.Models.ViewModels
{
    public class SprintView
    {
        public Sprint Sprint { get; set; }
        public List<ItemView> Items { get; set; }
    }
}