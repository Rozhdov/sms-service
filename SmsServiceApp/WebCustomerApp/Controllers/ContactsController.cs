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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using WebCustomerApp.Models.AccountViewModels;
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
        public IActionResult Add(WebCustomerApp.Models.ContactsViewModels.AddContactViewModel newContact, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                bool result = _contactManager.AddUserContact(userId, newContact);
                if(!result)
                {
                    ModelState.AddModelError(string.Empty, "Invalid contact");
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Contacts");
                }
            }
            return View();
        }

        [HttpGet]

    }
}
