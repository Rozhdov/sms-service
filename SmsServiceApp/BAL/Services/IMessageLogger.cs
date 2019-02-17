using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebCustomerApp.Services
{
    public interface IMessageLogger
    {
        void Log(string Sender, IEnumerable<string> Recievers, string Message, DateTime SendingTime);
    }
}
