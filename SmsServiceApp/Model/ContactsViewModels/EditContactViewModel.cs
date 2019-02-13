﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebCustomerApp.Models.ContactsViewModels
{
    public class EditContactViewModel
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Roles")]
        public IEnumerable<string> Groups { get; set; }
    }

}