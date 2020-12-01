using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SPM.Web.Models;

namespace SPM.Web.Data
{
    public static class SeedData
    {
        /// <summary>
        /// Function to create default roles
        /// </summary>
        /// <param name="roleManager">Role manager service</param>
        public static async void CreateDefaultRoles(RoleManager<Role> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Developer").Result)
            {
                await roleManager.CreateAsync(new Role
                {
                    Name = "Developer"
                });
            }

            if (!roleManager.RoleExistsAsync("Maintainer").Result)
            {
                await roleManager.CreateAsync(new Role
                {
                    Name = "Maintainer"
                });
            }

            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                await roleManager.CreateAsync(new Role
                {
                    Name = "Administrator"
                });
            }
        }
    }
}
