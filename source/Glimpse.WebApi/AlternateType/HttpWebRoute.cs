using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace Glimpse.WebApi.AlternateType
{
    public class HttpWebRoute: System.Web.Routing.RouteBase
    {
		public HttpWebRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler, IHttpRoute httpRoute) : base(url, defaults, constraints, dataTokens, routeHandler)
		{
			this.HttpRoute = httpRoute;
		}
        
        public IHttpRoute HttpRoute
		{
			get;
			private set;
		}
        
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            throw new NotImplementedException();
        }
        
        public override RouteData GetRouteData(System.Web.HttpContextBase httpContext)
        {
            throw new NotImplementedException();
        }
    }
} 