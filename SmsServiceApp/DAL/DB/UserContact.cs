using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.Models
{
    public class UserContact
    {
        public int Id { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ContactGroup> ContactGroups { get; set; }


        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string PhoneNumber { get; set; }
        public string Name { get; set; }
    }
}
