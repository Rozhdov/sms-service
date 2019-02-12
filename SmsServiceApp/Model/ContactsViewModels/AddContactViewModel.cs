using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace WebCustomerApp.Models.ContactsViewModels
{
    public class AddContactViewModel
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9\+\*\#]{3,13}$", ErrorMessage = "Incorrect phone number")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Roles")]
        public IEnumerable<string> Groups { get; set; }
    }

}
