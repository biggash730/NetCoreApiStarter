using Quartz;
using System;
using System.Threading.Tasks;

namespace NetCoreStarter.Services
{
    public class MessageProcessor
    {
        public async Task Send(IServiceProvider serviceProvider)
        {
            try
            {
                
            }
            catch (Exception) { }
        }

    }

    [DisallowConcurrentExecution]
    public class MessageProcessService : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        public MessageProcessService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await new MessageProcessor().Send(_serviceProvider);
        }
    }

}
