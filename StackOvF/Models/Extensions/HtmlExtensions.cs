using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace StackOvF.Models.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlNodeCollection GetElementsByClassName(this HtmlDocument doc, string className)
        {
            var ret = doc.DocumentNode.SelectNodes("//*[contains(@class,'" + className + "')]");
            return ret;
        }
    }
}
