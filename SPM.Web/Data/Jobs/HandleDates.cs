using System;
using System.Linq;
using Quartz;
using SPM.Web.Models;
using Task = System.Threading.Tasks.Task;

namespace SPM.Web.Data.Jobs
{
    public class HandleDates : IJob
    {
        private ApplicationDbContext _context;

        /// <summary>
        ///     Function implemented from Quartz.Net to execute the job
        /// </summary>
        /// <param name="context">Database context</param>
        public async Task Execute(IJobExecutionContext context)
        {
            // get Quartz scheduler context
            var schedulerContext = context.Scheduler.Context;

            // Pull the database context object from the scheduler context
            _context = (ApplicationDbContext) schedulerContext.Get("context");

            // Run the handle sprint dates function asynchronously
            await HandleSprintDates();
        }

        /// <summary>
        ///     Function to handle the checking of sprint status based on dates
        /// </summary>
        public async Task HandleSprintDates()
        {
            // Get list of all sprints that are not complete
            var sprints = _context.Sprints
                .Where(x => x.Status != SprintStatus.Complete)
                .ToList();

            // Iterate through sprints
            foreach (var sprint in sprints)
            {
                // If sprint is inactive & start date has passed
                if (sprint.Status == SprintStatus.Inactive && CheckStart(sprint))
                    // Set sprint to active
                    sprint.Status = SprintStatus.Active;

                // If sprint is active & end date has passed
                if (sprint.Status == SprintStatus.Active && CheckEnd(sprint))
                    // Set sprint to extended
                    sprint.Status = SprintStatus.Extended;

                // Save database changes
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        ///     Check if the sprint should be started
        /// </summary>
        /// <param name="sprint">Sprint to check</param>
        /// <returns>Boolean indicating if sprint should be started</returns>
        public bool CheckStart(Sprint sprint)
        {
            return sprint.StartDate >= DateTime.UtcNow;
        }

        /// <summary>
        ///     Check if sprint should be extended
        /// </summary>
        /// <param name="sprint">Sprint to check</param>
        /// <returns>Boolean indicating if sprint should be extended</returns>
        public bool CheckEnd(Sprint sprint)
        {
            return sprint.EndDate <= DateTime.Now;
        }
    }
}