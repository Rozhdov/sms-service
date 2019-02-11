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
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Roles")]
        public List<Tuple<bool, string>> Groups {get; set;}   

        public AddContactViewModel()
        {
            Groups = new List<Tuple<bool, string>>();
        }
    }

}
