using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.MailingViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using WebCustomerApp.Models;

namespace WebCustomerApp.Managers
{
    public interface IMailingManager : IDisposable
    {
        Task<IEnumerable<MailingViewModel>> GetMailings(string AppUserId, int Num);
        Task<IEnumerable<MailingListItemViewModel>> GetMailingList(string AppUserId, int Num);
        Task<MailingViewModel> FindMailing(string AppUserId, int MailingId);
        Task<MailingViewModel> GetEmptyMailing(string AppUserId);

        Task<bool> AddMailing(string AppUserId, MailingViewModel Mailing);
        Task<bool> EditMailing(string AppUserId, MailingViewModel Mailing);
        Task<bool> RemoveMailing(string AppUserId, int MailingId);
    }
}
