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

            var alternateHttpRoutes = new Dictionary<string, System.Web.Http.Routing.IHttpRoute>();

            using (var currentHttpRoutes = GlobalConfiguration.Configuration.Routes)
            {
                var currentRoutes = System.Web.Routing.RouteTable.Routes;
                
                using (currentRoutes.GetWriteLock())
                {
                    var mappedRoutes = (Dictionary<string, System.Web.Routing.RouteBase>)MappedRoutesField.GetValue(currentRoutes);
    
                    for (var i = 0; i < currentRoutes.Count; i++)
                    {
                        var originalObj = currentRoutes[i];
                        if (!(originalObj.GetType().ToString() == "System.Web.Http.WebHost.Routing.HttpWebRoute"))
                        {
                            continue;
                        }
    
                        var newObj = (System.Web.Http.Routing.IHttpRoute)null;
                        var mixins = new[] { RouteNameMixin.None() };
                        var routeName = string.Empty; 
                        if (mappedRoutes.ContainsValue(originalObj))
                        {
                            var pair = mappedRoutes.First(r => r.Value == originalObj);
                            routeName = pair.Key;
                            mixins = new[] { new RouteNameMixin(pair.Key) };
                        }
                        
                        System.Web.Http.Routing.IHttpRoute originalObjHttpRoute;
                        currentHttpRoutes.TryGetValue(routeName, out originalObjHttpRoute);
                        
                        if (alternateBaseImplementation.TryCreate(originalObjHttpRoute, out newObj, mixins))
                        {
                            if (!string.IsNullOrEmpty(routeName))
                            {
                                alternateHttpRoutes.Add(routeName, newObj);
    	                        //mappedRoutes[routeName] = newObj;
                            }
    
                            logger.Info(Resources.RouteSetupReplacedRoute, originalObj.GetType());
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(routeName))
                            {
                                alternateHttpRoutes.Add(routeName, originalObjHttpRoute);
                            }
                            
                            logger.Info(Resources.RouteSetupNotReplacedRoute, originalObj.GetType());
                        }
                    }
                }
                
                // The WebAPI HttpRouteCollection is converted to a HostedHttpRouteCollection,
                // which doesn't have a Remove() method (it throws an exception). So instead create
                // a local copy of all the HttpRoutes, and then use the Clear() and Add() methods
                // to update the HostedHttpRouteCollection with the proxied routes

                currentHttpRoutes.Clear();
                
                foreach(var altHttpRoute in alternateHttpRoutes)
                {
                    currentHttpRoutes.Add(altHttpRoute.Key, altHttpRoute.Value);
                }

            }
        }
    }
}
