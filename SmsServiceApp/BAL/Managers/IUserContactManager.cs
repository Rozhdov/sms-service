using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.ContactsViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebCustomerApp.Managers
{
    public interface IUserContactManager : IDisposable
    {
        IEnumerable<ContactListViewModel> GetUserContacts(string AppUserId);
        IEnumerable<ContactListViewModel> GetUserContacts(string AppUserId, int Num);
        SelectList GetAvailableContactGroups(string AppUserId);

        bool AddContactGroup(string AppUserId, AddContactGroupViewModel ContactGroup);

        bool AddUserContact(string AppUserId, AddContactViewModel Contact);
        bool EditUserContact(string AppUserId, AddContactViewModel Contact);

        bool RemoveUserContact(string AppUserId, string UserContactPhone);


    }
}
