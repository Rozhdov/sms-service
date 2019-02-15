using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.Models
{
    public class Group
    {
        public int Id { get; set; }
        public IList<ContactGroup> ContactGroups { get; set; }
        public IList<GroupMailing> GroupMailings { get; set; }

        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}
