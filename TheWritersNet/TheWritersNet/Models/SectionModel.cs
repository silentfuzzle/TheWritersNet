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
        [RegularExpression(StringKeys.BRACKET_REGEX, ErrorMessage = StringKeys.BRACKET_ERROR_MSG)]
        public string Title { get; set; }

        [Required]
        [RegularExpression(StringKeys.BRACKET_REGEX, ErrorMessage = StringKeys.BRACKET_ERROR_MSG)]
        public string Text { get; set; }

        [Required]
        public int Position { get; set; }

        public bool DisplayTitle { get; set; }
        public bool IsSelected { get; set; }
    }
}