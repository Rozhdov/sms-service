using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebCustomerApp.Models.MailingViewModels;
using WebCustomerApp.Services;

namespace WebCustomerApp.Jobs
{
    public class MailingScheduler
    {

        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<MailingSender>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("triggerMailing", "mainGroup")
                .StartNow()
                .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(1)
                .RepeatForever()).Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
