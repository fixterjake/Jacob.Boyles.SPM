using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPM.Web.Models
{
    public class TeamPermissions
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        public Permissions Permissions { get; set; }
    }

    public enum Permissions
    {
        Member,
        Maintainer,
        Administrator
    }
}
