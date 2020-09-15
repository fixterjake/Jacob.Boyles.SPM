using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPM.Web.Models
{
    public class TaskComment
    {
        [Key]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int MemberId { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
    }
}
