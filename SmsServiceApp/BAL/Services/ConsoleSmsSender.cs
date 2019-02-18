using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebCustomerApp.Services
{
    class ConsoleSmsSender : ISmsSender
    {
        async public Task Send(string Sender, string Reciever, string Message, string Key)
        {
            Console.WriteLine("Sender: {Sender}");
            Console.WriteLine("Reciever: {Reviever}");
            Console.WriteLine("Message: {Message}");
        }
    }
}
