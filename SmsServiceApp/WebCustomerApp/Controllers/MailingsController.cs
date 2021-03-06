﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebCustomerApp.Models.MailingViewModels;
using WebCustomerApp.Services;
using WebCustomerApp.Managers;

namespace WebApp.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class MailingsController : Controller
    {
        private readonly IMailingManager _mailingManager;

        public MailingsController(IMailingManager mailingManager)
        {
            _mailingManager = mailingManager;
        }

        [HttpGet]
        async public Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewBag.Mailings = await _mailingManager.GetMailings(userId, 20);
            ViewBag.MailingList = await _mailingManager.GetMailingList(userId, 20);
            var newMailing = await _mailingManager.GetEmptyMailing(userId);
            return View(newMailing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<IActionResult> Index(MailingViewModel NewMailing)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = await _mailingManager.AddMailing(userId, NewMailing);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Adding failed");
                    return RedirectToAction("Index", "Mailings");
                }
                else
                {
                    return RedirectToAction("Index", "Mailings");
                }
            }
            return RedirectToAction("Index", "Mailings");
        }

        async public Task<IActionResult> Remove(int MailingId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool result = await _mailingManager.RemoveMailing(userId, MailingId);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Removal failed");
                return RedirectToAction("Index", "Mailings");
            }
            else
            {
                return RedirectToAction("Index", "Mailings");
            }
        }
        
        [HttpGet]
        async public Task<IActionResult> Edit(int MailingId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            MailingViewModel mailingToEdit = await _mailingManager.FindMailing(userId, MailingId);
            if (mailingToEdit == null)
            {
                ModelState.AddModelError(string.Empty, "Failure");
                return RedirectToAction("Index", "Mailings");
            }
            else
            {
                return View(mailingToEdit);
            }
        }

        [HttpPost]
        async public Task<IActionResult> Edit(MailingViewModel EditedMailing)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = await _mailingManager.EditMailing(userId, EditedMailing);
                if (!result)
                {
                    MailingViewModel mailingToEdit = await _mailingManager.FindMailing(userId, EditedMailing.Id);
                    ModelState.AddModelError(string.Empty, "Adding failed");
                    return View(mailingToEdit);
                }
                else
                {
                    return RedirectToAction("Index", "Mailings");
                }
            }
            else
            {
                return RedirectToAction("Edit", "Mailings");
            }
        }
    }
}
