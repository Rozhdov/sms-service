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

            // searchin for recievers
            var recievers = (from cg in db.ContactGroups
                            where Mailing.GroupIds.Contains(cg.UserContactGroupId)
                            select cg.UserContact).Distinct();
            if (!recievers.Any())
                return false;

            // creation and population of message list
            var messages = new List<Message>();
            string[] rawTimes = Mailing.Times.Split(',');
            var times = (from rt in rawTimes
                         where Convert.ToDateTime(rt) > DateTime.UtcNow
                         select Convert.ToDateTime(rt)).Distinct();
            if (!times.Any())
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

            foreach (var time in times)
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
            if (AppUserId == null || Mailing.Text == null)
                return false;

            var editedMailing = (from m in db.Mailings
                                 where m.SenderId == AppUserId && m.Id == Mailing.Id
                                 select m).FirstOrDefault();

            if (editedMailing == null)
                return false;

            editedMailing.Text = Mailing.Text;
            editedMailing.Title = Mailing.Title;
            db.SaveChanges();
            return true;
        }

        public MailingViewModel FindMailing(string AppUserId, int MailingId)
        {
            if (AppUserId == null)
                return null;
            var mailing = (from m in db.Mailings
                         where m.Id == MailingId && m.SenderId == AppUserId
                         select m).FirstOrDefault();
            if (mailing == null)
                return null;
            else
            {
                var result = new MailingViewModel()
                {
                    Title = mailing.Title,
                    Text = mailing.Text,
                    Id = mailing.Id,
                    TimeOfCreation = mailing.DateOfCreation
                };
                return result;
            }
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
                            select m).Take(Num).OrderByDescending(m => m.DateOfCreation);

            var result = new List<MailingViewModel>();

            foreach (var iter in mailings)
            {
                result.Add(new MailingViewModel()
                {
                    Id = iter.Id,
                    Text = iter.Text,
                    Title = iter.Title,
                    TimeOfCreation = iter.DateOfCreation
                });
            }
            return result;
        }

        public bool RemoveMailing(string AppUserId, int MailingId)
        {
            if (AppUserId == null)
                return false;
            var mailing = (from m in db.Mailings
                           where m.Id == MailingId && m.SenderId == AppUserId
                           select m).FirstOrDefault();
            if (mailing == null)
                return false;
            else
            {
                db.Mailings.Remove(mailing);
                db.SaveChanges();
                return true;
            }

        }
    }
}
