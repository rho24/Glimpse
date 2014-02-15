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

            using (var currentHttpRoutes = GlobalConfiguration.Configuration.Routes)
            {
                currentHttpRoutes.Clear();
                
                var currentRoutes = System.Web.Routing.RouteTable.Routes;
                
                using (currentRoutes.GetWriteLock())
                {
                    var mappedRoutes = (Dictionary<string, System.Web.Routing.RouteBase>)MappedRoutesField.GetValue(currentRoutes);
                    
                    // The WebAPI HttpRouteCollection is converted to a HostedHttpRouteCollection, which contains
                    // a private RouteCollection field. HostedHttpRouteCollection is internal, so can't declare it as a static field in RouteInspector
                    var routeCollection = currentHttpRoutes.GetType().GetField("_routeCollection", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(currentHttpRoutes);
                    //var mappedHttpRoutes = (Dictionary<string,  Glimpse.WebApi.AlternateType.HttpWebRoute>)MappedRoutesField.GetValue(routeCollection);
    
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
                        
                        var originalObjHttpRoute = (System.Web.Http.Routing.IHttpRoute)originalObj.GetType().GetField("<HttpRoute>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(originalObj);

                        if (alternateBaseImplementation.TryCreate(originalObjHttpRoute, out newObj, mixins))
                        {
                            if (!string.IsNullOrEmpty(routeName))
                            {
                                //currentHttpRoutes.Replace(routeName, routeName, newObj);
    	                        //currentHttpRoutes.Remove(routeName);
    	                        currentHttpRoutes.Add(routeName, newObj);
                                //mappedRoutes[routeName] = newObj;
                            }
    
                            logger.Info(Resources.RouteSetupReplacedRoute, originalObj.GetType());
                        }
                        else
                        {
                            logger.Info(Resources.RouteSetupNotReplacedRoute, originalObj.GetType());
                        }
                    }
                }
            }
        }
    }
}
