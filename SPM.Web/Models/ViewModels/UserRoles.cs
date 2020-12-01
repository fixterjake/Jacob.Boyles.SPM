using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPM.Web.Models.ViewModels
{
    public class UserRoles
    {
        public User User { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
