using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace WebCustomerApp.Models.ContactsViewModels
{
    public class AddContactGroupViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Group")]
        public string Group;

        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        public string Description;
    }
}
