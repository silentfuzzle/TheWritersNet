using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheWritersNet
{
    public static class StringKeys
    {
        #region Controllers

        public const string PAGE_CONTROLLER = "Page";

        #endregion

        #region Actions

        public const string EDIT_FROM_ID = "EditFromID";

        #endregion

        #region Miscellaneous

        public const string BRACKET_REGEX = "^[^<>]*$";
        public const string BRACKET_ERROR_MSG = "Angle brackets aren't allowed.";

        #endregion
    }
}