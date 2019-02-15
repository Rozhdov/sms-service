using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.Models
{
    public class Time
    {
        public int Id { get; set; }
        public int MailingId { get; set; }
        public Mailing Mailing { get; set; }

        public DateTime TimeToSend { get; set; }
        public bool BeenSent { get; set; }

    }
}
