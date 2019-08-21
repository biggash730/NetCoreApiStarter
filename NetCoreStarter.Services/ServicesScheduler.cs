using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace NetCoreStarter.Services
{
    public class ServicesScheduler
    {
        public static async Task StartAsync()
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
                await scheduler.Start();

                var messageService = JobBuilder.Create<MessageProcessService>()
                    .WithIdentity("job1", "group1")
                    .Build();
                var msgTrigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
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
