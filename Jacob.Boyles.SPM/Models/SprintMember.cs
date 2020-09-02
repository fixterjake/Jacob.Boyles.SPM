using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jacob.Boyles.SPM.Models
{
    public class SprintMember
    {
        [Key]
        public int Id { get; set; }
        public int SprintId { get; set; }
        public int MemberId { get; set; }
    }
}
