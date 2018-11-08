using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWritersNetData.Models
{
    public class DBSectionModel
    {
        public int PageID { get; set; }
        public int SectionID { get; set; }
        public bool IsSelected { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Position { get; set; }
        public bool DisplayTitle { get; set; }
    }
}
