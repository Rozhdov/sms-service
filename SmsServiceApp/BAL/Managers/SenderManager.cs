using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models;
using WebCustomerApp.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCustomerApp.Models.MailingViewModels;

namespace WebCustomerApp.Managers
{
    class SenderManager : ISenderManager
    {
        private ApplicationDbContext db { get; }

        public SenderManager(ApplicationDbContext Db)
        {
            this.db = Db;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        async public Task<IEnumerable<Message>> GetMessagesToSend()
        {
            return await db.Messages.ToListAsync();
            
        }

        async public Task<bool> SetTimesAsSent(IEnumerable<int> TimeIds)
        {
            if (TimeIds == null)
                return false;
            var times = await db.Times.Where(t => TimeIds.Contains(t.Id)).ToListAsync();
            foreach(var t in times)
            {
                t.BeenSent = true;
            }
            await db.SaveChangesAsync();
            return true;
        }
    }
}
