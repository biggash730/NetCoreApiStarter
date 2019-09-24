using Quartz;
using Quartz.Spi;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreStarter.Services
{
    public class SystemJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SystemJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    } 
}
