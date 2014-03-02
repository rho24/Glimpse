using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glimpse.Core.Extensibility;
using Glimpse.WebApi.Extensibility;
using System.Web.Http;

namespace Glimpse.WebApi.Tab
{
    class Configuration : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.Configuration;

            var result = new List<KeyValuePair<string, string>>();

            //Properties
            result.Add(new KeyValuePair<string, string>("Include Error Detail Policy", WebApiConfig.IncludeErrorDetailPolicy.ToString()));

            // Filters
            result.AddRange(WebApiConfig.Filters.Select(f => new KeyValuePair<string, string>("Filter", f.GetType().ToString())));
            result.AddRange(WebApiConfig.Filters.Select(f => new KeyValuePair<string, string>("Filter Instance", f.Instance.GetType().ToString())));
            result.AddRange(WebApiConfig.Filters.Select(f => new KeyValuePair<string, string>("Filter Instance Allow Multiple", f.Instance.AllowMultiple.ToString())));

            // Formatters
            result.Add(new KeyValuePair<string, string>("Form Url Encoded Formatter", WebApiConfig.Formatters.FormUrlEncodedFormatter.GetType().ToString()));
            result.Add(new KeyValuePair<string, string>("Json Formatter", WebApiConfig.Formatters.JsonFormatter.GetType().ToString()));
            result.Add(new KeyValuePair<string, string>("Xml Formatter", WebApiConfig.Formatters.XmlFormatter.GetType().ToString()));

            var traceWtr = WebApiConfig.Services.GetTraceWriter();

            return WebApiConfig;
        }

        public override string Name
        {
            get { return "WebAPI Configuration"; }
        }
    }

    public class DependancyResolver : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.Configuration;

            return WebApiConfig.DependencyResolver;
        }

        public override string Name
        {
            get { return "WebAPI Dependancy Resolver"; }
        }
    }

    public class Initializer : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.Configuration;

            return WebApiConfig.Initializer;
        }

        public override string Name
        {
            get { return "WebAPI Initializer"; }
        }
    }

    public class MessageHandlers : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.Configuration;

            return WebApiConfig.MessageHandlers;
        }

        public override string Name
        {
            get { return "WebAPI MessageHandlers"; }
        }
    }

    public class ParameterBindingRules : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.Configuration;

            return WebApiConfig.ParameterBindingRules;
        }

        public override string Name
        {
            get { return "WebAPI ParameterBindingRules"; }
        }
    }

    public class Properties : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.Configuration;

            return WebApiConfig.Properties;
        }

        public override string Name
        {
            get { return "WebAPI Properties"; }
        }
    }

    public class Filters : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.Configuration;

            var result = new List<KeyValuePair<string, string>>();

            // Filters
            result.AddRange(WebApiConfig.Filters.Select(f => new KeyValuePair<string, string>("Filter", f.GetType().ToString())));
            result.AddRange(WebApiConfig.Filters.Select(f => new KeyValuePair<string, string>("Filter Instance", f.Instance.GetType().ToString())));
            result.AddRange(WebApiConfig.Filters.Select(f => new KeyValuePair<string, string>("Filter Instance Allow Multiple", f.Instance.AllowMultiple.ToString())));

            //return System.Web.Mvc.GlobalFilters.Filters;
            return result;
        }

        public override string Name
        {
            get { return "WebAPI Filters"; }
        }
    }

    public class Formatters : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.Configuration;

            var result = new List<KeyValuePair<string, string>>();

            // Formatters
            result.Add(new KeyValuePair<string, string>("Form Url Encoded Formatter", WebApiConfig.Formatters.FormUrlEncodedFormatter.GetType().ToString()));
            result.Add(new KeyValuePair<string, string>("Json Formatter", WebApiConfig.Formatters.JsonFormatter.GetType().ToString()));
            result.Add(new KeyValuePair<string, string>("Xml Formatter", WebApiConfig.Formatters.XmlFormatter.GetType().ToString()));


            return WebApiConfig.Formatters;
        }

        public override string Name
        {
            get { return "WebAPI Formatters"; }
        }
    }



    //public class Services : WebApiTab
    //{
    //    public override object GetData(ITabContext context)
    //    {
    //        var WebApiConfig = GlobalConfiguration.Configuration;

    //        return WebApiConfig.Services;
    //    }

    //    public override string Name
    //    {
    //        get { return "WebAPI Services"; }
    //    }
    //}

    public class DefaultHandler : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.DefaultHandler;

            return WebApiConfig;
        }

        public override string Name
        {
            get { return "WebAPI DefaultHandler"; }
        }
    }
    public class DefaultServer : WebApiTab
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.DefaultServer;

            return WebApiConfig;
        }

        public override string Name
        {
            get { return "WebAPI DefaultServer"; }
        }
    }
}
