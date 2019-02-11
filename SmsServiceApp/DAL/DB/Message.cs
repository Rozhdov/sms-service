using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int MailingId { get; set; }
        public Mailing Mailing { get; set; }

        public int RecieverId { get; set; }
        public UserContact UserContact { get; set; }

        public DateTime TimeOfSending { get; set; }

        public bool BeenSent { get; set; }
    }
}
