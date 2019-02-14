using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.ContactsViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebCustomerApp.Managers
{
    public interface IUserContactManager : IDisposable
    {
        //EditContactViewModel FindContact(string AppUserId, int UserContactId);

        IEnumerable<GroupViewModel> GetContactGroups(string AppUserId, int Num);
        GroupViewModel FindContactGroup(string AppUserId, int UserContactGroupId);
        //MultiSelectList GetAvailableGroups(string AppUserId);

        bool AddContactGroup(string AppUserId, GroupViewModel ContactGroup);
        bool EditContactGroup(string AppUserId, GroupViewModel ContactGroup);

        bool RemoveUserContact(string AppUserId, int UserContactId);
        bool RemoveContactGroup(string AppUserId, int UserContactGroupId);

        bool AddContact(string AppUserId, ContactViewModel Contact);
        bool EditContact(string AppUserId, ContactViewModel Contact);

        IEnumerable<ContactViewModel> GetContacts(string AppUserId, int Num);
        ContactViewModel FindContact(string AppUserId, int UserContactId);
        ContactViewModel GetEmptyContact(string AppUserId);

    }
}
