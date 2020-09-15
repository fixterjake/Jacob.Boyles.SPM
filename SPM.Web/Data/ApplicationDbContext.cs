using System;
using System.Collections.Generic;
using System.Text;
using Jacob.Boyles.SPM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SPM.Web.Models;

namespace SPM.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add data sets to entity framework

        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<SprintMember> SprintMembers { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemMember> ItemMembers { get; set; }
        public DbSet<ItemComment> ItemComments { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskMember> TaskMembers { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
    }
}
