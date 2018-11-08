using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TheWritersNet.Models
{
    public class SectionModel
    {
        public int PageID { get; set; }
        public int SectionID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string Position { get; set; }

        public bool DisplayTitle { get; set; }
    }
}