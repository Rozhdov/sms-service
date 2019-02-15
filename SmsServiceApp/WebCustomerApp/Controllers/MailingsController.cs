using System;
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
        public IActionResult Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewBag.Mailings = _mailingManager.GetMailings(userId, 20);
            var newMailing = _mailingManager.GetEmptyMailing(userId);
            return View(newMailing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(MailingViewModel NewMailing)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = _mailingManager.AddMailing(userId, NewMailing);
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

        public IActionResult Remove(int MailingId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool result = _mailingManager.RemoveMailing(userId, MailingId);
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
        

        [HttpPost]
        public IActionResult Mailing(MailingViewModel EditedMailing)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = _mailingManager.EditMailing(userId, EditedMailing);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Editing failed");
                    return RedirectToAction("Index", "Mailings");
                }
                else
                {
                    return RedirectToAction("Index", "Mailings");
                }
            }
            return RedirectToAction("Index", "Mailings");
        }
    }
}
