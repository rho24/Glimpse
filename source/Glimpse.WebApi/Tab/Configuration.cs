using Glimpse.Core.Extensibility;
using Glimpse.WebApi.Extensibility;
using Glimpse.WebApi.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Glimpse.WebApi.Tab
{
    public class Configuration : WebApiTab, ILayoutControl
    {
        public override object GetData(ITabContext context)
        {
            var WebApiConfig = GlobalConfiguration.Configuration;

            var filters = WebApiConfig.Filters.Select(f => new FilterModel { Type = f.Instance.GetType().ToString(), AllowMultiple = f.Instance.AllowMultiple, Scope = f.Scope.ToString() });

            var formatters = new List<FormatterModel>();

            formatters.Add(new FormatterModel{
                Name = "Form Url Encoded Formatter",
                SupportedMediaTypes = WebApiConfig.Formatters.FormUrlEncodedFormatter.SupportedMediaTypes.Select(m => m.MediaType).ToList(),
                MediaTypeMappings = WebApiConfig.Formatters.FormUrlEncodedFormatter.MediaTypeMappings
            });

            formatters.Add(new FormatterModel
            {
                Name = "Json Formatter",
                SupportedMediaTypes = WebApiConfig.Formatters.JsonFormatter.SupportedMediaTypes.Select(m => m.MediaType).ToList(),
                MediaTypeMappings = WebApiConfig.Formatters.JsonFormatter.MediaTypeMappings
            });

            formatters.Add(new FormatterModel
            {
                Name = "Xml Formatter",
                SupportedMediaTypes = WebApiConfig.Formatters.XmlFormatter.SupportedMediaTypes.Select(m => m.MediaType).ToList(),
                MediaTypeMappings = WebApiConfig.Formatters.XmlFormatter.MediaTypeMappings
            });

            var properties = new List<KeyValuePair<object, object>>();
            properties.Add(new KeyValuePair<object, object>("Include Error Detail Policy", WebApiConfig.IncludeErrorDetailPolicy.ToString()));

            var allSingleServices = WebApiConfig.Services.GetType()
            .GetField("_cacheSingle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(WebApiConfig.Services);

            var services = (allSingleServices as ConcurrentDictionary<Type, object>)
                                    .Where(x => x.Value != null)
                                    .OrderBy(x => x.Key.FullName)
                                    .Select(x => new ServiceModel { Name = x.Value.GetType().FullName, Type = x.Key.FullName });

            return new ConfigurationModel
            {
                Filters = filters,
                Formatters = formatters,
                Properties = properties,
                Services = services
            };
        }

        public override string Name
        {
            get { return "WebAPI Configuration"; }
        }

        public bool KeysHeadings
        {
            get { return true; }
        }
    }
}
