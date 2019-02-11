using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.Models
{
    public class ContactGroup
    {
        public int UserContactId { get; set; }
        public UserContact UserContact { get; set; }

        public int UserContactGroupId { get; set; }
        public UserContactGroup UserContactGroup { get; set; }

    }
}
