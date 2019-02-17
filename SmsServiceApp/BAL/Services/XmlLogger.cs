using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebCustomerApp.Services
{
    public class XmlLogger : IMessageLogger
    {
        private string saveLocation;

        public XmlLogger(string SaveLocation)
        {
            this.saveLocation = SaveLocation;
        }


        public void Log(string Sender, IEnumerable<string> Recievers, string Message, DateTime SendingTime)
        {
            XElement MessagesLog;
            try
            {
                MessagesLog = XElement.Load(saveLocation);
            }
            catch (System.IO.FileNotFoundException)
            {
                MessagesLog = new XElement("Messages");
            }

            MessagesLog.Add(new XElement("Message",
                                   new XElement("Sender", Sender),
                                   new XElement("Recievers", Recievers),
                                   new XElement("Text", Message),
                                   new XAttribute("Date_and_time", Convert.ToString(SendingTime))
                                   ));
            MessagesLog.Save(saveLocation);
        }
    }
}
