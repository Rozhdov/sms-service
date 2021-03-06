﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebCustomerApp.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<Group> Groups { get; set; }
        public ICollection<Mailing> Mailings { get; set; }
    }
}
