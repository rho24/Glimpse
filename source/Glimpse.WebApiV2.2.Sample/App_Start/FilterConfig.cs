﻿using System.Web;
using System.Web.Mvc;

namespace Glimpse.WebApiV2.Sample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
