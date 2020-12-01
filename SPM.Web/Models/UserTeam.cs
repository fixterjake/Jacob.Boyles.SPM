using System;
using System.ComponentModel.DataAnnotations;

namespace SPM.Web.Models
{
    public class UserTeam
    {
        [Key] public int Id { get; set; }

        public int UserId { get; set; }
        public int TeamId { get; set; }
        public DateTime Created { get; set; }
    }
}