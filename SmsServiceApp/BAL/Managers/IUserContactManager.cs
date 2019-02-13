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

        EditContactViewModel FindContact(string AppUserId, int UserContactId);

        IEnumerable<ContactGroupListViewModel> GetContactGroups(string AppUserId);
        IEnumerable<ContactGroupListViewModel> GetContactGroups(string AppUserId, int Num);
        SelectList GetAvailableContactGroups(string AppUserId);
        EditContactGroupViewModel FindContactGroup(string AppUserId, int UserContactGroupId);

        bool AddContactGroup(string AppUserId, AddContactGroupViewModel ContactGroup);
        bool EditContactGroup(string AppUserId, EditContactGroupViewModel ContactGroup);

        bool AddUserContact(string AppUserId, AddContactViewModel Contact);
        bool EditUserContact(string AppUserId, EditContactViewModel Contact);

        bool RemoveUserContact(string AppUserId, int UserContactId);
        bool RemoveContactGroup(string AppUserId, int UserContactGroupId);


    }
}
