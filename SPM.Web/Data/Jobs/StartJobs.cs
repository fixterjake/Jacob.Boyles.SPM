﻿using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace SPM.Web.Data.Jobs
{
    public class StartJobs
    {
        /// <summary>
        ///     Function to start all jobs for the backend
        /// </summary>
        /// <param name="context">Database context</param>
        public static async Task StartAllJobs(ApplicationDbContext context)
        {
            try
            {
                // Setup quartz job service
                var props = new NameValueCollection
                {
                    {"quartz.serializer.type", "binary"}
                };
                var factory = new StdSchedulerFactory(props);
                var scheduler = await factory.GetScheduler();

                // Add database context to scheduler context
                scheduler.Context.Put("context", context);

                // Create handle dates job
                var handleDatesJob = JobBuilder.Create<HandleDates>()
                    .WithIdentity("HandleDatesJob", "Jobs")
                    .Build();

                // Create datafile trigger
                var handleDatesTrigger = TriggerBuilder.Create()
                    .WithIdentity("HandleDatesTrigger", "Jobs")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInHours(12)
                        .RepeatForever())
                    .Build();

                // Add job to scheduler
                await scheduler.ScheduleJob(handleDatesJob, handleDatesTrigger);

                // Start the scheduler
                await scheduler.Start();
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Starting jobs encountered an exception: {ex.Message}");
            }
        }
    }
}