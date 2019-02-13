using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.ContactsViewModels;
using WebCustomerApp.Models;
using WebCustomerApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace WebCustomerApp.Managers
{
    // ToDo: remade LINQ queries with React Framework or PLINQ for async
    // Methods with bool return type can be remade with operation result class return type

    public class UserContactManager : IUserContactManager
    {
        private ApplicationDbContext db { get; }

        public UserContactManager(ApplicationDbContext Db)
        {
            this.db = Db;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public bool RemoveUserContact(string AppUserId, int UserContactId)
        {
            if (AppUserId == null)
                return false;

            UserContact userContact = (from uc in db.UserContacts
                                       where uc.Id == UserContactId && uc.UserId == AppUserId
                                       select uc).FirstOrDefault();
            if (userContact == null)
                return false;
            db.UserContacts.Remove(userContact);
            db.SaveChanges();
            return true;
        }

        public bool AddContactGroup(string AppUserId, AddContactGroupViewModel ContactGroup)
        {
            if (AppUserId == null || ContactGroup.Group == null)
                return false;
            var temp = (from ucg in db.UserContactGroups
                        where ucg.Group == ContactGroup.Group && ucg.UserId == AppUserId
                        select ucg).FirstOrDefault();
            if (temp != null)
                return false;
            var newGroup = new UserContactGroup()
            {
                UserId = AppUserId,
                Group = ContactGroup.Group,
                Description = ContactGroup.Description
            };
            db.UserContactGroups.Add(newGroup);
            db.SaveChanges();
            return true;
        }

        public IEnumerable<ContactGroupListViewModel> GetContactGroups(string AppUserId)
        {
            if (AppUserId == null)
            {
                return null;
            }
            var groups = from g in db.UserContactGroups
                         where g.UserId == AppUserId
                         select g;
            var result = new List<ContactGroupListViewModel>();
            foreach (var iter in groups)
            {
                result.Add(new ContactGroupListViewModel()
                {
                    Id = iter.Id,
                    Group = iter.Group,
                    Description = iter.Description
                });
            }
            return result;

        }

        public IEnumerable<ContactGroupListViewModel> GetContactGroups(string AppUserId, int Num)
        {
            if (AppUserId == null)
            {
                return null;
            }
            var groups = (from g in db.UserContactGroups
                          where g.UserId == AppUserId
                          select g).Take(Num);
            var result = new List<ContactGroupListViewModel>();
            foreach (var iter in groups)
            {
                result.Add(new ContactGroupListViewModel()
                {
                    Id = iter.Id,
                    Group = iter.Group,
                    Description = iter.Description
                });
            }
            return result;
        }

        public bool EditContactGroup(string AppUserId, EditContactGroupViewModel ContactGroup)
        {
            if (AppUserId == null || ContactGroup.Group == null || db.UserContactGroups.Any(ucg => ucg.Group == ContactGroup.Group))
                return false;

            var updatedContactGroup = (from cg in db.UserContactGroups
                                       where cg.UserId == AppUserId && cg.Id == ContactGroup.Id
                                       select cg).FirstOrDefault();

            if (updatedContactGroup == null)
                return false;

            updatedContactGroup.Group = ContactGroup.Group;
            updatedContactGroup.Description = ContactGroup.Description;

            db.SaveChanges();
            return true;
        }

        public bool RemoveContactGroup(string AppUserId, int UserContactGroupId)
        {
            if (AppUserId == null)
                return false;
            var ContactGroupToRemove = db.UserContactGroups.Find(UserContactGroupId);
            if (ContactGroupToRemove == null || ContactGroupToRemove.UserId != AppUserId)
                return false;
            db.UserContactGroups.Remove(ContactGroupToRemove);
            db.SaveChanges();
            return true;
        }

        public EditContactGroupViewModel FindContactGroup(string AppUserId, int UserContactGroupId)
        {
            if (AppUserId == null)
                return null;
            var group = (from uc in db.UserContactGroups
                         where uc.Id == UserContactGroupId && uc.UserId == AppUserId
                         select uc).FirstOrDefault();
            if (group == null)
                return null;
            else
            {
                var result = new EditContactGroupViewModel()
                {
                    Group = group.Group,
                    Description = group.Description,
                    Id = group.Id
                };
                return result;
            }
        }

        public bool AddContact(string AppUserId, ContactViewModel Contact)
        {
            // check on required fields
            if (AppUserId == null || Contact.PhoneNumber == null)
                return false;
            // check for duplicate
            var check = from uc in db.UserContacts
                        where uc.PhoneNumber == Contact.PhoneNumber && uc.UserId == AppUserId
                        select uc;
            if (check.Any())
                return false;
            var newContact = new UserContact()
            {
                UserId = AppUserId,
                PhoneNumber = Contact.PhoneNumber,
                Name = Contact.Name
            };
            db.UserContacts.Add(newContact);

            // adding contact to selected groups
            if (Contact.GroupIds != null)
                foreach (int iter in Contact.GroupIds)
                {
                    // check, if group exist
                    var group = (from ucg in db.UserContactGroups
                                 where ucg.Id == iter && ucg.UserId == AppUserId
                                 select ucg).FirstOrDefault();
                    if (group != null)
                    {
                        ContactGroup cg = new ContactGroup()
                        {
                            UserContact = newContact,
                            UserContactGroup = group
                        };
                        db.ContactGroups.Add(cg);
                    }
                }
            db.SaveChanges();
            return true;
        }

        public bool EditContact(string AppUserId, ContactViewModel Contact)
        {
            // Check for required fields
            if (AppUserId == null || Contact.PhoneNumber == null)
                return false;

            // Check of existance and authority
            var updatedContact = (from uc in db.UserContacts.Include(uc => uc.ContactGroups)
                                  where uc.Id == Contact.Id && uc.UserId == AppUserId
                                  select uc).FirstOrDefault();
            if (updatedContact == null)
                return false;

            // Check for duplicate phone number
            var temp = (from uc in db.UserContacts
                        where uc.PhoneNumber == Contact.PhoneNumber && uc.UserId == AppUserId && uc.Id != Contact.Id
                        select uc).FirstOrDefault();
            if (temp == null)
                return false;

            updatedContact.Name = Contact.Name;
            updatedContact.PhoneNumber = Contact.PhoneNumber;

            // Removing contact from every group 
            var groups = from cg in db.ContactGroups
                         where cg.UserContact == updatedContact
                         select cg;
            foreach (var iter in groups)
            {
                updatedContact.ContactGroups.Remove(iter);
            }
            db.SaveChanges();

            // Adding contact to selected groups
            if (Contact.GroupIds != null)
            {
                foreach (int iter in Contact.GroupIds)
                {
                    // check, if group exist
                    var group = (from ucg in db.UserContactGroups
                                 where ucg.Id == iter && ucg.UserId == AppUserId
                                 select ucg).FirstOrDefault();
                    if (group != null)
                    {
                        ContactGroup cg = new ContactGroup()
                        {
                            UserContact = updatedContact,
                            UserContactGroup = group
                        };
                        updatedContact.ContactGroups.Add(cg);
                    }
                }
            }
            db.SaveChanges();
            return true;
        }

        public IEnumerable<ContactViewModel> GetContacts(string AppUserId, int Num)
        {
            var contacts = (from uc in db.UserContacts
                            .Include(uc => uc.ContactGroups)
                            .ThenInclude(cg => cg.UserContactGroup)
                            where uc.UserId == AppUserId
                            select uc).Take(Num);
            var result = new List<ContactViewModel>();
            foreach (var iter in contacts)
            {
                result.Add(new ContactViewModel()
                {
                    Id = iter.Id,
                    Name = iter.Name,
                    PhoneNumber = iter.PhoneNumber,
                });

                var selectedGroups = from cg in iter.ContactGroups
                                     select cg.UserContactGroup.Id;

                var availableGroups = from ucg in db.UserContactGroups
                                      where ucg.UserId == AppUserId
                                      select ucg;
                result.Last().Groups = new MultiSelectList(availableGroups, "Id", "Group", selectedGroups);
            }
            return result;
        }

        public ContactViewModel FindContact(string AppUserId, int UserContactId)
        {
            if (AppUserId == null)
                return null;

            var contact = (from uc in db.UserContacts
                           .Include(uc => uc.ContactGroups)
                           .ThenInclude(cg => cg.UserContactGroup)
                           where uc.Id == UserContactId && uc.UserId == AppUserId
                           select uc).FirstOrDefault();
            if (contact == null)
                return null;
            else
            {
                var result = new ContactViewModel()
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    PhoneNumber = contact.PhoneNumber,
                };

                var selectedGroups = from cg in contact.ContactGroups
                                     select cg.UserContactGroup.Id;

                var availableGroups = from ucg in db.UserContactGroups
                                      where ucg.UserId == AppUserId
                                      select ucg;

                result.Groups = new MultiSelectList(availableGroups, "Id", "Group", selectedGroups);
                return result;
            }
        }

        public ContactViewModel GetEmptyContact(string AppUserId)
        {
            if (AppUserId == null)
                return null;
            var result = new ContactViewModel();
            var availableGroups = from ucg in db.UserContactGroups
                                  where ucg.UserId == AppUserId
                                  select ucg;

            result.Groups = new MultiSelectList(availableGroups, "Id", "Group");
            return result;
        }
    }
}

