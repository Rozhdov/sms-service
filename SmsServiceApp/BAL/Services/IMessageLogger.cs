﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebCustomerApp.Services
{
    public interface IMessageLogger
    {
        void Log(string Sender, string Reciever, string Message, DateTime SendingTime);
    }
}
