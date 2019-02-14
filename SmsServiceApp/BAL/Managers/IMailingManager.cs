using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.MailingViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebCustomerApp.Managers
{
    public interface IMailingManager : IDisposable
    {
        IEnumerable<MailingViewModel> GetMailings(string AppUserId, int Num);
        MailingViewModel FindMailing(string AppUserId, int MailingId);
        MailingViewModel GetEmptyMailing(string AppUserId);

        bool AddMailing(string AppUserId, MailingViewModel Mailing);
        bool EditMailing(string AppUserId, MailingViewModel Mailing);
        bool RemoveMailing(string AppUserId, int MailingId);
    }
}
