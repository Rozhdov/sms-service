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
using WebCustomerApp.Models.ContactsViewModels;
using WebCustomerApp.Services;
using WebCustomerApp.Managers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ContactsController : Controller
    {
        private readonly IUserContactManager _contactManager;

        public ContactsController(IUserContactManager userContactManager)
        {
            _contactManager = userContactManager;
        }
        
        // GET: /<controller>/
        [HttpGet]
        async public Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewBag.Contacts = await _contactManager.GetContacts(userId, 20);
            var newContact = await _contactManager.GetEmptyContact(userId);
            return View(newContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<IActionResult> Index(ContactViewModel newContact)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = await _contactManager.AddContact(userId, newContact);
                if(!result)
                {
                    ModelState.AddModelError(string.Empty, "Adding failed");
                    ViewBag.Contacts = await _contactManager.GetContacts(userId, 20);
                    return View(newContact);
                }
                else
                {
                    return RedirectToAction("Index", "Contacts");
                }
            }
            return View();
        }

        async public Task<IActionResult> RemoveContact(int UserContactId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool result = await _contactManager.RemoveUserContact(userId, UserContactId);
            if(!result)
            {
                ModelState.AddModelError(string.Empty, "Delete failed");
                return RedirectToAction("Index", "Contacts");
            }
            else
            {
                return RedirectToAction("Index", "Contacts");
            }
        }
              
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        async public Task<IActionResult> Contact(ContactViewModel UserContact)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await _contactManager.EditContact(userId, UserContact);
            if (ModelState.IsValid)
            {
                if (result == false)
                {
                    ModelState.AddModelError(string.Empty, "Modify failed");
                    return RedirectToAction("Index", "Contacts");
                }
                else
                {
                    return RedirectToAction("Index", "Contacts"); ;
                }
            }
            return RedirectToAction("Index", "Contacts");
        }

        [HttpGet]
        async public Task<IActionResult> Groups()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewBag.Groups = await _contactManager.GetContactGroups(userId, 20);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<IActionResult> Groups(GroupViewModel newGroup)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = await _contactManager.AddContactGroup(userId, newGroup);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Invalid contact");
                    return RedirectToAction("Groups", "Contacts");
                }
                else
                {
                    return RedirectToAction("Groups", "Contacts");
                }
            }
            return View();
        }

        async public Task<IActionResult> RemoveContactGroup(int ContactGroupId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool result = await _contactManager.RemoveContactGroup(userId, ContactGroupId);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Delete failed");
                return RedirectToAction("Groups", "Contacts");
            }
            else
            {
                return RedirectToAction("Groups", "Contacts");
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        async public Task<IActionResult> Group(GroupViewModel UserContactGroup)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await _contactManager.EditContactGroup(userId, UserContactGroup);
            if (ModelState.IsValid)
            {
                if (result == false)
                {
                    ModelState.AddModelError(string.Empty, "Modify failed");
                    return RedirectToAction("Groups", "Contacts");
                }
                else
                {
                    return RedirectToAction("Groups", "Contacts");
                }
            }
            return RedirectToAction("Groups", "Contacts");
        }

    }
}
