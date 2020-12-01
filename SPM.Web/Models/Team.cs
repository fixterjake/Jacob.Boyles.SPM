using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SPM.Web.Models
{
    public class Team
    {
        [Key] public int Id { get; set; }

        public int CreatorId { get; set; }

        [Required(ErrorMessage = "Please enter a name for the team.")]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a description for your team.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string Image { get; set; }

        [NotMapped]
        [Display(Name = "Team Image")]
        public IFormFile FormImage { get; set; }

        public DateTime Created { get; set; }
    }
}