using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebCustomerApp.Models.ContactsViewModels
{
    public class ContactViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9\+\*\#]{3,13}$", ErrorMessage = "Incorrect phone number")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Groups")]
        public int[] GroupIds { get; set; }

        [Display(Name = "Groups")]
        public MultiSelectList Groups { get; set; }
    }

}
