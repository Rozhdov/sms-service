using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.ContactsViewModels;
using WebCustomerApp.Models;
using WebCustomerApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
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

        async public Task<bool> RemoveUserContact(string AppUserId, int UserContactId)
        {
            if (AppUserId == null)
                return false;

            Contact userContact = await (from uc in db.Contacts
                                       where uc.Id == UserContactId && uc.UserId == AppUserId
                                       select uc).FirstOrDefaultAsync();
            if (userContact == null)
                return false;
            db.Contacts.Remove(userContact);
            await db.SaveChangesAsync();
            return true;
        }

        async public Task<bool> AddContactGroup(string AppUserId, GroupViewModel ContactGroup)
        {
            if (AppUserId == null || ContactGroup.Group == null)
                return false;
            var temp = await (from ucg in db.Groups
                        where ucg.Title == ContactGroup.Group && ucg.UserId == AppUserId
                        select ucg).FirstOrDefaultAsync();
            if (temp != null)
                return false;
            var newGroup = new Group()
            {
                UserId = AppUserId,
                Title = ContactGroup.Group,
                Description = ContactGroup.Description
            };
            await db.Groups.AddAsync(newGroup);
            await db.SaveChangesAsync();
            return true;
        }


        async public Task<IEnumerable<GroupViewModel>> GetContactGroups(string AppUserId, int Num)
        {
            if (AppUserId == null)
            {
                return null;
            }
            var groups = await (from g in db.Groups
                          where g.UserId == AppUserId
                          select g).Take(Num).ToListAsync();
            var result = new List<GroupViewModel>();
            foreach (var iter in groups)
            {
                result.Add(new GroupViewModel()
                {
                    Id = iter.Id,
                    Group = iter.Title,
                    Description = iter.Description
                });
            }
            return result;
        }

        async public Task<bool> EditContactGroup(string AppUserId, GroupViewModel ContactGroup)
        {
            if (AppUserId == null || ContactGroup.Group == null)
                return false;

            var contactGroups = await (from cg in db.Groups
                                       where cg.UserId == AppUserId
                                       select cg).ToListAsync();

            // Check for duplicate group title
            if (contactGroups.Any(cg => cg.Title == ContactGroup.Group && cg.Id != ContactGroup.Id))
                return false;

            var updatedContactGroup = (from cg in contactGroups
                                       where cg.Id == ContactGroup.Id
                                       select cg).FirstOrDefault();

            if (updatedContactGroup == null)
                return false;

            updatedContactGroup.Title = ContactGroup.Group;
            updatedContactGroup.Description = ContactGroup.Description;

            await db.SaveChangesAsync();
            return true;
        }

        async public Task<bool> RemoveContactGroup(string AppUserId, int UserContactGroupId)
        {
            if (AppUserId == null)
                return false;
            var ContactGroupToRemove = await db.Groups.FindAsync(UserContactGroupId);
            if (ContactGroupToRemove == null || ContactGroupToRemove.UserId != AppUserId)
                return false;
            db.Groups.Remove(ContactGroupToRemove);
            await db.SaveChangesAsync();
            return true;
        }

        async public Task<GroupViewModel> FindContactGroup(string AppUserId, int UserContactGroupId)
        {
            if (AppUserId == null)
                return null;
            var group = await (from uc in db.Groups
                         where uc.Id == UserContactGroupId && uc.UserId == AppUserId
                         select uc).FirstOrDefaultAsync();
            if (group == null)
                return null;
            else
            {
                var result = new GroupViewModel()
                {
                    Group = group.Title,
                    Description = group.Description,
                    Id = group.Id
                };
                return result;
            }
        }

        async public Task<bool> AddContact(string AppUserId, ContactViewModel Contact)
        {
            // check on required fields
            if (AppUserId == null || Contact.PhoneNumber == null)
                return false;
            // check for duplicate
            var check = await (from uc in db.Contacts
                        where uc.PhoneNumber == Contact.PhoneNumber && uc.UserId == AppUserId
                        select uc).FirstOrDefaultAsync();
            if (check != null)
                return false;
            var newContact = new Contact()
            {
                UserId = AppUserId,
                PhoneNumber = Contact.PhoneNumber,
                Name = Contact.Name
            };
            await db.Contacts.AddAsync(newContact);

            // adding contact to selected groups
            if (Contact.GroupIds != null)
            {
                var groups = await (from ucg in db.Groups
                                    where ucg.UserId == AppUserId
                                    select ucg).ToListAsync();

                foreach (int iter in Contact.GroupIds)
                {
                    // check, if group exist
                    var group = (from g in groups
                                 where g.Id == iter
                                 select g).FirstOrDefault();
                    if (group != null)
                    {
                        ContactGroup cg = new ContactGroup()
                        {
                            Contact = newContact,
                            Group = group
                        };
                        await db.ContactGroups.AddAsync(cg);
                    }
                }
            }
            await db.SaveChangesAsync();
            return true;
        }

        async public Task<bool> EditContact(string AppUserId, ContactViewModel Contact)
        {
            // Check for required fields
            if (AppUserId == null || Contact.PhoneNumber == null)
                return false;

            var userContacts = await (from uc in db.Contacts
                                      where uc.UserId == AppUserId
                                      select uc).ToListAsync();

            // Check of existance and authority
            var updatedContact = (from uc in userContacts
                                  where uc.Id == Contact.Id
                                  select uc).FirstOrDefault();

            if (updatedContact == null)
                return false;

            // Check for duplicate phone number
            var temp = (from uc in userContacts
                        where uc.PhoneNumber == Contact.PhoneNumber && uc.Id != Contact.Id
                        select uc).FirstOrDefault();
            if (temp != null)
                return false;

            updatedContact.Name = Contact.Name;
            updatedContact.PhoneNumber = Contact.PhoneNumber;

            // Removing contact from every group 
            var contactGroups = await (from uc in db.ContactGroups
                                       where uc.ContactId == updatedContact.Id
                                       select uc).ToListAsync();
            foreach (var iter in contactGroups)
            {
                updatedContact.ContactGroups.Remove(iter);
            }
            await db.SaveChangesAsync();

            // Adding contact to selected groups
            if (Contact.GroupIds != null)
            {
                var groups = await (from ucg in db.Groups
                              where ucg.UserId == AppUserId
                              select ucg).ToListAsync();
                foreach (int iter in Contact.GroupIds)
                {
                    // check, if group exist
                    var group = (from ucg in groups
                                 where ucg.Id == iter
                                 select ucg).FirstOrDefault();
                    if (group != null)
                    {
                        ContactGroup cg = new ContactGroup()
                        {
                            Contact = updatedContact,
                            Group = group
                        };
                        updatedContact.ContactGroups.Add(cg);
                    }
                }
            }
            await db.SaveChangesAsync();
            return true;
        }

        async public Task<IEnumerable<ContactViewModel>> GetContacts(string AppUserId, int Num)
        {
            var contacts = await (from uc in db.Contacts
                            .Include(uc => uc.ContactGroups)
                            .ThenInclude(cg => cg.Group)
                            where uc.UserId == AppUserId
                            select uc).Take(Num).ToListAsync();
            var result = new List<ContactViewModel>();

            var availableGroups = await (from ucg in db.Groups
                                  where ucg.UserId == AppUserId
                                  select ucg).ToListAsync();
            foreach (var iter in contacts)
            {
                result.Add(new ContactViewModel()
                {
                    Id = iter.Id,
                    Name = iter.Name,
                    PhoneNumber = iter.PhoneNumber,
                });

                var selectedGroups = from cg in iter.ContactGroups
                                     select cg.Group.Id;

                result.Last().Groups = new MultiSelectList(availableGroups, "Id", "Title", selectedGroups);
            }
            return result;
        }

        async public Task<ContactViewModel> FindContact(string AppUserId, int UserContactId)
        {
            if (AppUserId == null)
                return null;

            var contact = await (from uc in db.Contacts
                           .Include(uc => uc.ContactGroups)
                           .ThenInclude(cg => cg.Group)
                           where uc.Id == UserContactId && uc.UserId == AppUserId
                           select uc).FirstOrDefaultAsync();
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
                                     select cg.Group.Id;

                var availableGroups = await (from ucg in db.Groups
                                      where ucg.UserId == AppUserId
                                      select ucg).ToListAsync();

                result.Groups = new MultiSelectList(availableGroups, "Id", "Title", selectedGroups);
                return result;
            }
        }

        async public Task<ContactViewModel> GetEmptyContact(string AppUserId)
        {
            if (AppUserId == null)
                return null;
            var result = new ContactViewModel();
            var availableGroups = await (from ucg in db.Groups
                                  where ucg.UserId == AppUserId
                                  select ucg).ToListAsync();

            result.Groups = new MultiSelectList(availableGroups, "Id", "Title");
            return result;
        }

    }
}

