using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.MailingViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using WebCustomerApp.Models;

namespace WebCustomerApp.Managers
{
    interface ISenderManager : IDisposable
    {
        // Methods for sending service

        Task<IEnumerable<Message>> GetMessagesToSend();
        Task<bool> SetTimesAsSent(IEnumerable<int> TimeIds);
    }
}
