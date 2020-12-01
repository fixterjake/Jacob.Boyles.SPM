using System;
using System.ComponentModel.DataAnnotations;

namespace SPM.Web.Models
{
    public class UserItem
    {
        [Key] public int Id { get; set; }

        public int UserId { get; set; }
        public int ItemId { get; set; }
        public DateTime Created { get; set; }
    }
}