using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jacob.Boyles.SPM.Models
{
    public class ItemComment
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int MemberId { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
    }
}
