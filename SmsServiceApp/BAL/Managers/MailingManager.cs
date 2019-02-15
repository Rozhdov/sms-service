using System;
using System.Collections.Generic;
using System.Text;
using WebCustomerApp.Models;
using WebCustomerApp.Data;
using System.Linq;
using System.Threading.Tasks;
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

        async public Task<bool> AddMailing(string AppUserId, MailingViewModel Mailing)
        {
            // check on required fields
            if (AppUserId == null || Mailing.Text == null || Mailing.GroupIds == null || Mailing.Times == null)
                return false;

            // searchin for reciever groups
            var groups = await (from g in db.Groups
                            where Mailing.GroupIds.Contains(g.Id) && g.UserId == AppUserId
                            select g).ToListAsync();
            if (!groups.Any())
                return false;

            // creation and population of Times list
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
                DateOfCreation = DateTime.UtcNow
            };

            // populating group - mailings
            var groupMailings = new List<GroupMailing>();
            foreach (var iter in groups)
            {
                groupMailings.Add(new GroupMailing()
                {
                    Group = iter,
                    Mailing = newMailing
                });
            }

            // creating sending times
            var sendingTimes = new List<Time>();
            foreach (var time in times)
            {
                sendingTimes.Add(new Time()
                {
                    TimeToSend = time,
                    Mailing = newMailing,
                    BeenSent = false,
                });      
            }

            await db.Mailings.AddAsync(newMailing);
            await db.Times.AddRangeAsync(sendingTimes);
            await db.GroupMailings.AddRangeAsync(groupMailings);
            await db.SaveChangesAsync();
            return true;
        }

        async public Task<bool> EditMailing(string AppUserId, MailingViewModel Mailing)
        {
            if (AppUserId == null || Mailing.Text == null)
                return false;

            var editedMailing = await (from m in db.Mailings
                                 where m.SenderId == AppUserId && m.Id == Mailing.Id
                                 select m).FirstOrDefaultAsync();

            if (editedMailing == null)
                return false;

            editedMailing.Text = Mailing.Text;
            editedMailing.Title = Mailing.Title;
            await db.SaveChangesAsync();
            return true;
        }

        async public Task<MailingViewModel> FindMailing(string AppUserId, int MailingId)
        {
            if (AppUserId == null)
                return null;
            var mailing = await (from m in db.Mailings
                         where m.Id == MailingId && m.SenderId == AppUserId
                         select m).FirstOrDefaultAsync();
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

        async public Task<MailingViewModel> GetEmptyMailing(string AppUserId)
        {
            if (AppUserId == null)
                return null;
            var result = new MailingViewModel();
            var availableGroups = await (from g in db.Groups
                                  where g.UserId == AppUserId
                                  select g).ToListAsync();

            result.Groups = new MultiSelectList(availableGroups, "Id", "Title");
            return result;
        }

        async public Task<IEnumerable<MailingViewModel>> GetMailings(string AppUserId, int Num)
        {
            var mailings = await (from m in db.Mailings
                            where m.SenderId == AppUserId
                            select m).Take(Num).OrderByDescending(m => m.DateOfCreation).ToListAsync();

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

        async public Task<bool> RemoveMailing(string AppUserId, int MailingId)
        {
            if (AppUserId == null)
                return false;
            var mailing = await (from m in db.Mailings
                           where m.Id == MailingId && m.SenderId == AppUserId
                           select m).FirstOrDefaultAsync();
            if (mailing == null)
                return false;
            else
            {
                db.Mailings.Remove(mailing);
                await db.SaveChangesAsync();
                return true;
            }

        }
    }
}
