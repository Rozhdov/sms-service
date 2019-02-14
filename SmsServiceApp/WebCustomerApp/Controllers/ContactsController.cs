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
        public IActionResult Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewBag.Contacts = _contactManager.GetContacts(userId, 20);
            var newContact = _contactManager.GetEmptyContact(userId);
            return View(newContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ContactViewModel newContact)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = _contactManager.AddContact(userId, newContact);
                if(!result)
                {
                    ModelState.AddModelError(string.Empty, "Adding failed");
                    ViewBag.Contacts = _contactManager.GetContacts(userId, 20);
                    return View(newContact);
                }
                else
                {
                    return RedirectToAction("Index", "Contacts");
                }
            }
            return View();
        }

        public IActionResult RemoveContact(int UserContactId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool result = _contactManager.RemoveUserContact(userId, UserContactId);
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
        public IActionResult Contact(ContactViewModel UserContact)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = _contactManager.EditContact(userId, UserContact);
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
        public IActionResult Groups()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewBag.Groups = _contactManager.GetContactGroups(userId, 20);
            var temp = _contactManager.GetContactGroups(userId, 20);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Groups(GroupViewModel newGroup)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = _contactManager.AddContactGroup(userId, newGroup);
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

        public IActionResult RemoveContactGroup(int ContactGroupId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool result = _contactManager.RemoveContactGroup(userId, ContactGroupId);
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
        public IActionResult Group(GroupViewModel UserContactGroup)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = _contactManager.EditContactGroup(userId, UserContactGroup);
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
