using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SPM.Web.Models;

namespace SPM.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add data sets to entity framework

        public DbSet<Item> Items { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<UserSprint> UserSprints { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
    }
}