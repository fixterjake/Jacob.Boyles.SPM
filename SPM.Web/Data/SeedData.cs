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
        /// Function to add default application settings
        /// </summary>
        /// <param name="context">Database context</param>
        public static async void SeedSettings(ApplicationDbContext context)
        {
            try
            {
                // Check if settings already exist
                if (!context.Settings.Any())
                {
                    // Add various settings to the database context
                    await context.Settings.AddAsync(new Settings
                    {
                        Name = "FirstTimeSetup",
                        Value = true.ToString()
                    });

                    await context.Settings.AddAsync(new Settings
                    {
                        Name = "AzureConnectionString"
                    }); ;

                    // Save database changes
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

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
