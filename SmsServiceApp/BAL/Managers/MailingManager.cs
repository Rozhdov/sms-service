using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models;
using WebCustomerApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCustomerApp.Models.MailingViewModels;

namespace WebCustomerApp.Managers
{
    public class MailingManager : IMailingManager
    {
        private ApplicationDbContext db { get; }

        public MailingManager(ApplicationDbContext Db)
        {
            this.db = Db;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public bool AddMailing(string AppUserId, MailingViewModel Mailing)
        {
            // check on required fields
            if (AppUserId == null || Mailing.Text == null || Mailing.GroupIds == null)
                return false;

            // creating new mailing
            var newMailing = new Mailing()
            {
                SenderId = AppUserId,
                Title = Mailing.Title,
                Text = Mailing.Text,
                DateOfCreation = DateTime.Now
            };
            db.Mailings.Add(newMailing);

            // searchin for recievers
            var recievers = from cg in db.ContactGroups
                            where Mailing.GroupIds.Contains(cg.UserContactGroupId)
                            select cg.UserContact;

            // creation and population of message list
            var messages = new List<Message>();
            foreach (var time in Mailing.Times)
            {
                foreach (var reciever in recievers)
                {
                    messages.Add(new Message()
                    {
                        UserContact = reciever,
                        TimeOfSending = time,
                        Mailing = newMailing,
                        BeenSent = false,
                    });      
                }
            }

            db.Messages.AddRange(messages);
            db.SaveChanges();
            return true;
        }

        public bool EditMailing(string AppUserId, MailingViewModel Mailing)
        {
            throw new NotImplementedException();
        }

        public MailingViewModel FindMailing(string AppUserId, int MailingId)
        {
            throw new NotImplementedException();
        }

        public MailingViewModel GetEmptyMailing(string AppUserId)
        {
            if (AppUserId == null)
                return null;
            var result = new MailingViewModel();
            var availableGroups = from ucg in db.UserContactGroups
                                  where ucg.UserId == AppUserId
                                  select ucg;

            result.Groups = new MultiSelectList(availableGroups, "Id", "Group");
            return result;
        }

        public IEnumerable<MailingViewModel> GetMailings(string AppUserId, int Num)
        {
            var mailings = (from m in db.Mailings
                            where m.SenderId == AppUserId
                            select m).Take(Num);

            var result = new List<MailingViewModel>();

            foreach (var iter in mailings)
            {
                result.Add(new MailingViewModel()
                {
                    Id = iter.Id,
                    Text = iter.Text,
                    Title = iter.Text,
                    TimeOfCreation = iter.DateOfCreation
                });
            }
            return result;
        }

        public bool RemoveMailing(string AppUserId, int MailingId)
        {
            throw new NotImplementedException();
        }
    }
}
