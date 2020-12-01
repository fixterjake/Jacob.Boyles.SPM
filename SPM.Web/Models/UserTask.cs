using System;
using System.ComponentModel.DataAnnotations;

namespace SPM.Web.Models
{
    public class UserTask
    {
        [Key] public int Id { get; set; }

        public int UserId { get; set; }
        public int TaskId { get; set; }
        public DateTime Created { get; set; }
    }
}