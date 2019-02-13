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

        public bool AddUserContact(string AppUserId, AddContactViewModel Contact)
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
            if (Contact.Groups != null)
            foreach (string iter in Contact.Groups)
            {
                    // check, if group exist
                    var group = (from ucg in db.UserContactGroups
                                 where ucg.Group == iter && ucg.UserId == AppUserId
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



        public void Dispose()
        {
            db.Dispose();
        }


        public bool EditUserContact(string AppUserId, EditContactViewModel Contact)
        {
            if (AppUserId == null || Contact.PhoneNumber == null)
                return false;

            var updatedContact = (from uc in db.UserContacts
                                  where uc.PhoneNumber == Contact.PhoneNumber && uc.UserId == AppUserId
                                  select uc).FirstOrDefault();
            if (updatedContact == null)
                return false;

            updatedContact.Name = Contact.Name;

            // Removing contact from every group 
            var groups = from cg in db.ContactGroups
                         where cg.UserContact == updatedContact
                         select cg;            
            foreach (var iter in groups)
            {
                updatedContact.ContactGroups.Remove(iter);
            }

            // Adding contact to selected groups
            if (Contact.Groups != null)
            {
                foreach (string iter in Contact.Groups)
                {
                    // check, if group exist
                    var group = (from ucg in db.UserContactGroups
                                 where ucg.Group == iter && ucg.UserId == AppUserId
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

        public IEnumerable<ContactListViewModel> GetUserContacts(string AppUserId)
        {
            var contacts = from uc in db.UserContacts
                           .Include(uc => uc.ContactGroups)
                           .ThenInclude(cg => cg.UserContactGroup)
                           where uc.UserId == AppUserId
                           select uc;
            var result = new List<ContactListViewModel>();
            foreach (var iter in contacts)
            {
                result.Add(new ContactListViewModel()
                {
                    Id = iter.Id,
                    Name = iter.Name,
                    PhoneNumber = iter.PhoneNumber,
                    Groups = ""                    
                });
                if (iter.ContactGroups != null)
                {
                    List<string> groups = new List<string>();
                    foreach (var jter in iter.ContactGroups)
                    {
                        groups.Add(jter.UserContactGroup.Group);
                    }
                    result.Last().Groups = String.Join(", ", groups);
                }
            }
            return result;
        }

        public IEnumerable<ContactListViewModel> GetUserContacts(string AppUserId, int Num)
        {
            var contacts = (from uc in db.UserContacts
                            .Include(uc => uc.ContactGroups)
                            .ThenInclude(cg => cg.UserContactGroup)                           
                            where uc.UserId == AppUserId
                            select uc).Take(Num);
            var result = new List<ContactListViewModel>();
            foreach (var iter in contacts)
            {
                result.Add(new ContactListViewModel()
                {
                    Id = iter.Id,
                    Name = iter.Name,
                    PhoneNumber = iter.PhoneNumber,
                    Groups = ""
                });

                if (iter.ContactGroups != null)
                {
                    List<string> groups = new List<string>();
                    foreach (var jter in iter.ContactGroups)
                    {
                        groups.Add(jter.UserContactGroup.Group);
                    }
                    result.Last().Groups = String.Join(", ", groups);
                }
            }
            return result;
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


        public SelectList GetAvailableContactGroups(string AppUserId)
        {
            if (AppUserId == null)
            {
                return null;
            }
            var groups = from g in db.UserContactGroups
                         where g.UserId == AppUserId
                         select g;
            SelectList result = new SelectList(groups, "Group", "Group");
            return result;
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

        public EditContactViewModel FindContact(string AppUserId, int UserContactId)
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
                var temp = from cg in contact.ContactGroups
                           select cg.UserContactGroup.Group;

                var result = new EditContactViewModel()
                {
                    PhoneNumber = contact.PhoneNumber,
                    Name = contact.Name,
                    Groups = temp
                };
                return result;               
            }
        }

        public bool EditContactGroup(string AppUserId, EditContactGroupViewModel ContactGroup)
        {
            if (AppUserId == null || ContactGroup.Group == null)
                return false;

            var updatedContactGroup = db.UserContactGroups.Find(ContactGroup.Id);
            if (updatedContactGroup == null || updatedContactGroup.UserId != AppUserId)
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
    }
}
