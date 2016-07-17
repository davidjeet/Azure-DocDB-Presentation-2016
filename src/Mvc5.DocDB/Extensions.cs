using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc5.DocDB
{
    public static class Extensions
    {

        public static List<string> StringToList(this string commaSeparatedList)
        {
            return commaSeparatedList.Split(',').Select(s => s.ToLower().Trim()).ToList();
        }

        public static string ListToString(this List<string> list)
        {
            return string.Join(",", list);
        }
    }
}