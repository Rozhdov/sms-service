using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace WebCustomerApp.Models.ContactsViewModels
{
    public class ContactGroupListViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Group")]
        public string Group { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Description")]
        public string Description { get; set; }

    }
}
