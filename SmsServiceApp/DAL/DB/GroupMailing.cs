using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.Models
{
    public class GroupMailing
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int MailingId { get; set; }
        public Mailing Mailing { get; set; }
    }
}
