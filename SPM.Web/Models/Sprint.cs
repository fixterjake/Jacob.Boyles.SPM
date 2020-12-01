using System;
using System.ComponentModel.DataAnnotations;

namespace SPM.Web.Models
{
    public class Sprint
    {
        [Key] public int Id { get; set; }

        [Display(Name = "Team Id")] public int TeamId { get; set; }

        [Required(ErrorMessage = "Please enter a sprint name.")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a starting date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Please enter an ending date.")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Please select an initial status.")]
        public SprintStatus Status { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        ///     Returns string value of sprint status.
        /// </summary>
        /// <returns>String value of status</returns>
        public string GetStatusString()
        {
            return Status switch
            {
                SprintStatus.Inactive => "Inactive",
                SprintStatus.Active => "Active",
                SprintStatus.Extended => "Extended",
                SprintStatus.Complete => "Complete",
                _ => throw new ArgumentOutOfRangeException()
            };
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