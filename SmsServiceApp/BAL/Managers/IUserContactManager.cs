using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.ContactsViewModels;

namespace WebCustomerApp.Managers
{
    public interface IUserContactManager : IDisposable
    {
        IEnumerable<ContactListViewModel> GetUserContacts(string AppUserId);
        IEnumerable<ContactListViewModel> GetUserContacts(string AppUserId, int Num);

        bool AddUserContact(string AppUserId, AddContactViewModel Contact);
        bool EditUserContact(string AppUserId, AddContactViewModel Contact);

        bool RemoveUserContact(string AppUserId, string UserContactPhone);

    }
}
