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
            ViewBag.Contacts = _contactManager.GetUserContacts(userId, 20);
            ViewBag.Groups = _contactManager.GetAvailableContactGroups(userId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AddContactViewModel newContact)
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = _contactManager.AddUserContact(userId, newContact);
                if(!result)
                {
                    ModelState.AddModelError(string.Empty, "Adding failed");
                    return RedirectToAction("Index", "Contacts");
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

        [HttpGet]
        public IActionResult ChangeContact(int UserContactId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var contact = _contactManager.FindContact(userId, UserContactId);
            ViewBag.Groups = _contactManager.GetAvailableContactGroups(userId);
            if (contact == null)
            {
                ModelState.AddModelError(string.Empty, "Modify failed");
                return RedirectToAction("Index", "Contacts");
            }
            else
            {
                return View(contact);
            }
        }
        
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ChangeContact(EditContactViewModel UserContact)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = _contactManager.EditUserContact(userId, UserContact);
            if (ModelState.IsValid)
            {
                if (result == false)
                {
                    ModelState.AddModelError(string.Empty, "Modify failed");
                    return View(UserContact);
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
        public IActionResult Groups(AddContactGroupViewModel newGroup)
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


        [HttpGet]
        public IActionResult ChangeContactGroup(int UserContactGroupId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var group = _contactManager.FindContactGroup(userId, UserContactGroupId);
            if (group == null)
            {
                ModelState.AddModelError(string.Empty, "Modify failed");
                return RedirectToAction("Groups", "Contacts");
            }
            else
            {
                return View(group);
            }
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ChangeContactGroup(EditContactGroupViewModel UserContactGroup)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = _contactManager.EditContactGroup(userId, UserContactGroup);
            if (ModelState.IsValid)
            {
                if (result == false)
                {
                    ModelState.AddModelError(string.Empty, "Modify failed");
                    return View(UserContactGroup);
                }
                else
                {
                    return RedirectToAction("Groups", "Contacts"); ;
                }
            }
            return RedirectToAction("Groups", "Contacts");
        }


    }
}
