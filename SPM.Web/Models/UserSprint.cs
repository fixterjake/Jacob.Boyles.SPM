using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPM.Web.Models
{
    public class UserSprint
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SprintId { get; set; }
        public DateTime Created { get; set; }
    }
}
