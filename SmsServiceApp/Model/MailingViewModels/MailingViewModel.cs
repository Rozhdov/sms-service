using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebCustomerApp.Models.MailingViewModels
{
    public class MailingViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Text")]
        public string Text { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Time of creation")]
        public DateTime TimeOfCreation { get; set; }

        [Required]
        [Display(Name = "Groups")]
        public int[] GroupIds { get; set; }

        [Display(Name = "Groups")]
        public MultiSelectList Groups { get; set; }

        [Required]
        [Display(Name = "Times")]
        public string Times { get; set; }

    }
}
