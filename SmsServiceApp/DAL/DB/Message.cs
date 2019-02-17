using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.DB
{
    public class Message
    {
        public string Sender { get; private set; }
        public string Reciever { get; private set; }
        public string Text { get; private set; }
    }    
}
