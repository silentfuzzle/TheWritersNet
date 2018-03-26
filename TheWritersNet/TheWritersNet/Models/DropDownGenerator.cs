using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheWritersNet.Models
{
    public static class DropDownGenerator
    {
        public static List<SelectListItem> GetVisibilityDropDown(int selected)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem
            {
                Text = "Public",
                Value = "1"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Private",
                Value = "2"
            });

            if (selected < listItems.Count && selected >= 0)
                listItems[selected].Selected = true;
            else
                listItems[0].Selected = true;

            return listItems;
        }
    }
}