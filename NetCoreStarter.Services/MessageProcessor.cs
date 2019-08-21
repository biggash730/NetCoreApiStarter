using Quartz;
using System;
using System.Threading.Tasks;

namespace NetCoreStarter.Services
{
    public class MessageProcessor
    {
        public async Task Send()
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
        public async Task Execute(IJobExecutionContext context)
        {
            await new MessageProcessor().Send();
        }
    }

}
