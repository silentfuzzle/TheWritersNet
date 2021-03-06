﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheWritersNetData.Models;
using TheWritersNetData.DBConnectors;

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

        public static List<SelectListItem> GetPermissionDropDown(int selected)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem
            {
                Text = "Owner",
                Value = "1"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Admin",
                Value = "2"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Writer",
                Value = "3"
            });
            listItems.Add(new SelectListItem
            {
                Text = "Viewer",
                Value = "4"
            });

            if (selected < listItems.Count && selected >= 0)
                listItems[selected].Selected = true;
            else
                listItems[0].Selected = true;

            return listItems;
        }

        public static List<SelectListItem> GetSocialMediaDropDown(int selected)
        {
            IDBConnector db = DBConnectorFactory.GetDBConnector();
            List<SocialMediaModel> models = db.SelectSocialMediaOptions();

            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (SocialMediaModel model in models)
            {
                listItems.Add(new SelectListItem
                {
                    Text = model.Name,
                    Value = model.SocialMediaID.ToString(),
                    Selected = (model.SocialMediaID == selected)
                });
            }

            return listItems;
        }
    }
}