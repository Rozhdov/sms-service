using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace WebCustomerApp.Jobs
{
    public class MailingSender : IJob
    {
        async public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Vzhuh");
        }
    }
}
