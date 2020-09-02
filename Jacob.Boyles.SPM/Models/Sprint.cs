using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jacob.Boyles.SPM.Models
{
    public class Sprint
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SprintStatus Status { get; set; }
        public DateTime Created { get; set; }

        /// <summary>
        /// Returns string value of sprint status.
        /// </summary>
        /// <returns>String value of status</returns>
        public string GetStatusString()
        {
            switch(Status)
            {
                case SprintStatus.Inactive:
                    return "Inactive";

                case SprintStatus.Active:
                    return "Active";

                case SprintStatus.Extended:
                    return "Extended";

                case SprintStatus.Complete:
                    return "Complete";

                default: 
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    public enum SprintStatus
    {
        Inactive,
        Active,
        Extended,
        Complete
    }
}
