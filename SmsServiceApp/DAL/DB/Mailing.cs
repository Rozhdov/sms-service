using System;
using System.Collections.Generic;
using System.Text;

namespace WebCustomerApp.Models
{
    public class Mailing
    {
        public int Id { get; set; }
        public ICollection<Message> Messages { get; set; }

        public string SenderId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime DateOfCreation { get; set; }
    }
}
