using System;
using System.ComponentModel.DataAnnotations;

namespace SPM.Web.Models
{
    public class Item
    {
        [Key] public int Id { get; set; }

        public int SprintId { get; set; }

        [Required(ErrorMessage = "Please enter an item name.")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter an item description.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public bool Estimated { get; set; }

        [Required(ErrorMessage = "Please enter an item name.")]
        [Display(Name = "Status")]
        public ItemStatus Status { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        ///     Returns string value of item status.
        /// </summary>
        /// <returns>String value of status</returns>
        public string GetStatusString()
        {
            return Status switch
            {
                ItemStatus.Pending => "Pending",
                ItemStatus.InProgress => "In Progress",
                ItemStatus.Blocked => "Blocked",
                ItemStatus.Complete => "Complete",
                _ => throw new ArgumentOutOfRangeException("")
            };
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