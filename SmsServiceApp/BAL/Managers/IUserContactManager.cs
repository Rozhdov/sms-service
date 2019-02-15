using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.ContactsViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace WebCustomerApp.Managers
{
    public interface IUserContactManager : IDisposable
    {

        //IEnumerable<GroupViewModel> GetContactGroups(string AppUserId, int Num);
        //GroupViewModel FindContactGroup(string AppUserId, int UserContactGroupId);

        //bool AddContactGroup(string AppUserId, GroupViewModel ContactGroup);
        //bool EditContactGroup(string AppUserId, GroupViewModel ContactGroup);

        //bool RemoveUserContact(string AppUserId, int UserContactId);
        //bool RemoveContactGroup(string AppUserId, int UserContactGroupId);

        //bool AddContact(string AppUserId, ContactViewModel Contact);
        //bool EditContact(string AppUserId, ContactViewModel Contact);

        //IEnumerable<ContactViewModel> GetContacts(string AppUserId, int Num);
        //ContactViewModel FindContact(string AppUserId, int UserContactId);
        //ContactViewModel GetEmptyContact(string AppUserId);

        Task<IEnumerable<GroupViewModel>> GetContactGroups(string AppUserId, int Num);
        Task<GroupViewModel> FindContactGroup(string AppUserId, int UserContactGroupId);

        Task<bool> AddContactGroup(string AppUserId, GroupViewModel ContactGroup);
        Task<bool> EditContactGroup(string AppUserId, GroupViewModel ContactGroup);

        Task<bool> RemoveUserContact(string AppUserId, int UserContactId);
        Task<bool> RemoveContactGroup(string AppUserId, int UserContactGroupId);

        Task<bool> AddContact(string AppUserId, ContactViewModel Contact);
        Task<bool> EditContact(string AppUserId, ContactViewModel Contact);

        Task<IEnumerable<ContactViewModel>> GetContacts(string AppUserId, int Num);
        Task<ContactViewModel> FindContact(string AppUserId, int UserContactId);
        Task<ContactViewModel> GetEmptyContact(string AppUserId);

    }
}
