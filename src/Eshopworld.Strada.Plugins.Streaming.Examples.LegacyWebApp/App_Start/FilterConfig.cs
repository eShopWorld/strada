﻿using System.Web;
using System.Web.Mvc;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.LegacyWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
