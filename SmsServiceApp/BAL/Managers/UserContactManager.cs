using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models.ContactsViewModels;
using WebCustomerApp.Models;
using WebCustomerApp.Data;
using System.Linq;

namespace WebCustomerApp.Managers
{
    // ToDo: remade LINQ queries with React Framework for async
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
            var newContact = new UserContact()
            {
                UserId = AppUserId,
                PhoneNumber = Contact.PhoneNumber,
                Name = Contact.Name
            };
            db.UserContacts.Add(newContact);
            foreach (Tuple<bool, string> iter in Contact.Groups)
            {
                if (iter.Item1)
                {
                    // check, if group exist
                    var group = (from ucg in db.UserContactGroups
                                 where ucg.Group == iter.Item2 && ucg.UserId == AppUserId
                                 select ucg).FirstOrDefault();
                    if (group != null)
                    {
                        ContactGroup cg = new ContactGroup()
                        {
                            UserContactId = newContact.Id,
                            UserContactGroupId = group.Id
                        };
                        newContact.ContactGroups.Add(cg);
                    }
                }
            }
            db.SaveChanges();
            return true;
        }



        public void Dispose()
        {
            db.Dispose();
        }

        public bool EditUserContact(string AppUserId, AddContactViewModel Contact)
        {
            if (AppUserId == null || Contact.PhoneNumber == null)
                return false;

            var updatedContact = (from uc in db.UserContacts
                                  where uc.PhoneNumber == Contact.PhoneNumber && uc.UserId == AppUserId
                                  select uc).FirstOrDefault();
            if (updatedContact == null)
                return false;

            updatedContact.Name = Contact.Name;
            foreach (Tuple<bool, string> iter in Contact.Groups)
            {
                if (iter.Item1)
                {
                    // check, if group exist
                    var group = (from ucg in db.UserContactGroups
                                 where ucg.Group == iter.Item2 && ucg.UserId == AppUserId
                                 select ucg).FirstOrDefault();
                    if (group != null)
                    {
                        ContactGroup cg = new ContactGroup()
                        {
                            UserContactId = updatedContact.Id,
                            UserContactGroupId = group.Id
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
                           where uc.UserId == AppUserId
                           select uc;
            var result = new List<ContactListViewModel>();
            foreach (var iter in contacts)
            {
                result.Add(new ContactListViewModel()
                {
                    Name = iter.Name,
                    PhoneNumber = iter.PhoneNumber,
                    Groups = ""                    
                });
                foreach (var jter in iter.ContactGroups)
                {
                    result.Last().Groups += jter.UserContactGroup.Group;
                    result.Last().Groups += ", ";
                }
            }
            return result;
        }

        public IEnumerable<ContactListViewModel> GetUserContacts(string AppUserId, int Num)
        {
            var contacts = (from uc in db.UserContacts
                           where uc.UserId == AppUserId
                           select uc).Take(Num);
            var result = new List<ContactListViewModel>();
            foreach (var iter in contacts)
            {
                result.Add(new ContactListViewModel()
                {
                    Name = iter.Name,
                    PhoneNumber = iter.PhoneNumber,
                    Groups = ""
                });
                foreach (var jter in iter.ContactGroups)
                {
                    result.Last().Groups += jter.UserContactGroup.Group;
                    result.Last().Groups += ", ";
                }
            }
            return result;
        }

        public bool RemoveUserContact(string AppUserId, string UserContactPhone)
        {
            if (AppUserId == null || UserContactPhone == null)
                return false;

            UserContact userContact = (from uc in db.UserContacts
                                       where uc.PhoneNumber == UserContactPhone && uc.UserId == AppUserId
                                       select uc).FirstOrDefault();
            if (userContact == null)
                return false;

            db.UserContacts.Remove(userContact);
            db.SaveChanges();
            return true;
        }
    }
}
