using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebCustomerApp.Services
{
    interface ISmsSender
    {
        Task Send(string Sender, string Reciever, string Message, string Key);
    }
}
