using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Glimpse.WebApi.AlternateType;
using Glimpse.Core.Extensibility;

namespace Glimpse.WebApi.Inspector
{
    public class RoutesInspector : IInspector
    {
        private static readonly FieldInfo MappedRoutesField = typeof(System.Web.Routing.RouteCollection).GetField("_namedMap", BindingFlags.NonPublic | BindingFlags.Instance);
        
        public void Setup(IInspectorContext context)
        {
            var logger = context.Logger;
            var alternateBaseImplementation = new AlternateType.IHttpRoute(context.ProxyFactory, context.Logger); 

            using (var currentRoutes = GlobalConfiguration.Configuration.Routes)
            {
                // The WebAPI HttpRouteCollection is converted to a HostedHttpRouteCollection, which contains
                // a private RouteCollection field. HostedHttpRouteCollection is internal, so can't declare it as a static field in RouteInspector
                var routeCollection = currentRoutes.GetType().GetField("_routeCollection", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(currentRoutes);
                var mappedRoutes = (Dictionary<string,  Glimpse.WebApi.AlternateType.HttpWebRoute>)MappedRoutesField.GetValue(routeCollection);

                for (var i = 0; i < currentRoutes.Count; i++)
                {
                    var originalObj = currentRoutes[i];
                    if (!(typeof(System.Web.Http.Routing.IHttpRoute)).IsAssignableFrom(originalObj.GetType()))
                    {
                        continue;
                    }

                    var newObj = (System.Web.Http.Routing.IHttpRoute)null;
                    var mixins = new[] { RouteNameMixin.None() };
                    var routeName = string.Empty; 
//                    if (mappedRoutes.ContainsValue(originalObj))
//                    {
//                        var pair = mappedRoutes.First(r => r.Value == originalObj);
//                        routeName = pair.Key;
//                        mixins = new[] { new RouteNameMixin(pair.Key) };
//                    }
//                      
//                    if (alternateBaseImplementation.TryCreate(originalObj, out newObj, mixins))
//                    {
//                        if (!string.IsNullOrEmpty(routeName))
//                        {
//	                        currentRoutes.Remove(routeName);
//	                        currentRoutes.Add(routeName, newObj);
//                            mappedRoutes[routeName] = newObj;
//                        }
//
//                        logger.Info(Resources.RouteSetupReplacedRoute, originalObj.GetType());
//                    }
//                    else
//                    {
//                        logger.Info(Resources.RouteSetupNotReplacedRoute, originalObj.GetType());
//                    }
                }
            }
        }
    }
}
