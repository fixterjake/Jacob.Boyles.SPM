using System.ComponentModel.DataAnnotations;

namespace SPM.Web.Models.ViewModels
{
    public class Role
    {
        [Required(ErrorMessage = "Please select a user.")]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        [Display(Name = "Role")]
        public int RoleId { get; set; }
    }
}