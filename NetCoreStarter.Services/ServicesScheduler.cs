using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace NetCoreStarter.Services
{
    public static class ServicesScheduler
    {
        public static async Task StartAsync(IServiceProvider serviceProvider)
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();
                scheduler.JobFactory = new SystemJobFactory(serviceProvider);
                await scheduler.Start();

                var messageService = JobBuilder.Create<MessageProcessService>()
                    .WithIdentity("net_core_starter_job_1", "net_core_starter_group_1")
                    .Build();
                var msgTrigger = TriggerBuilder.Create()
                    .WithIdentity("net_core_starter_trigger_1", "net_core_starter_group_1")
                        .StartNow()
                        .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(5)
                            .RepeatForever())
                        .Build();

                await scheduler.ScheduleJob(messageService, msgTrigger);
            }
            catch (SchedulerException ex)
            {

            }
        } 
    }
}
