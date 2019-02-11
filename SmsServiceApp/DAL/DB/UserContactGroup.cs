using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.Models
{
    public class UserContactGroup
    {
        public int Id { get; set; }
        public IList<ContactGroup> ContactGroups { get; set; }

        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string Group { get; set; }
        public string Description { get; set; }
    }
}
