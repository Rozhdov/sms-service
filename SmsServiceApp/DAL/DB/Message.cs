using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.Models
{
    public class Message
    {
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public string Text { get; set; }
        public int TimeId { get; set; }
    }    
}
