using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPM.Web.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ItemId { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime Created { get; set; }

        /// <summary>
        /// Returns string value of task status.
        /// </summary>
        /// <returns>String value of status</returns>
        public string GetStatusString()
        {
            switch (Status)
            {
                case TaskStatus.Pending:
                    return "Pending";

                case TaskStatus.InProgress:
                    return "In Progress";

                case TaskStatus.Blocked:
                    return "Blocked";

                case TaskStatus.Complete:
                    return "Complete";

                default:
                    throw new ArgumentOutOfRangeException("");
            }
        }
    }

    public enum TaskStatus
    {
        Pending,
        InProgress,
        Blocked,
        Complete
    }
}
