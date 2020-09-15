using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPM.Web.Models
{
    public class TeamMember
    {
        [Key]
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int MemberId { get; set; }
    }
}
