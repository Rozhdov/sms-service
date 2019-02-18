using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Autofac;
using WebCustomerApp.Data;
using WebCustomerApp.Models;
using WebCustomerApp.Services;
using Microsoft.EntityFrameworkCore;
using WebCustomerApp.Managers;
using Quartz;
using System.Linq;

namespace WebCustomerApp.Jobs
{
    public class MailingSender : IJob
    {
        async public Task Execute(IJobExecutionContext context)
        {
            DbContextOptionsBuilder<ApplicationDbContext> optionbuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionbuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SmsServiceAppDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            ApplicationDbContext db = new ApplicationDbContext(optionbuilder.Options);

            var senderManager = new SenderManager(db);
            var messageLogger = new XmlLogger("XmlLog.xml");
            var smsSender = new ConsoleSmsSender();

            var messages = await senderManager.GetMessagesToSend();
            foreach (var message in messages)
            {
                await smsSender.Send(message.Sender, message.Reciever, message.Text, "");
                messageLogger.Log(message.Sender, message.Reciever, message.Text, DateTime.UtcNow);
            }
            var Ids = from i in messages
                        select i.TimeId;
            await senderManager.SetTimesAsSent(Ids);
                
        }
    }
}
