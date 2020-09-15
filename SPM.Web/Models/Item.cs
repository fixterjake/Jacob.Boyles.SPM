using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jacob.Boyles.SPM.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public int SprintId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
        public ItemStatus Status { get; set; }
        public DateTime Created { get; set; }

        /// <summary>
        /// Returns string value of item status.
        /// </summary>
        /// <returns>String value of status</returns>
        public string GetStatusString()
        {
            switch (Status) 
            {
                case ItemStatus.Pending:
                    return "Pending";

                case ItemStatus.InProgress:
                    return "In Progress";

                case ItemStatus.Blocked:
                    return "Blocked";

                case ItemStatus.Complete:
                    return "Complete";

                default:
                    throw new ArgumentOutOfRangeException("");
            }
        }
    }

    public enum ItemStatus
    {
        Pending,
        InProgress,
        Blocked,
        Complete
    }
}
