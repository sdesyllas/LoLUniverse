using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoLUniverse.Models
{
    public class NewsModel
    {
        private NewsModel()
        {
            
        }

        public int NewsId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Created on")]
        public DateTime CreatedDate { get; set; }
    }
}