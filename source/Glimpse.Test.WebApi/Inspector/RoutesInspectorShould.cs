using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Http.Routing;
using Glimpse.WebApi.AlternateType;
using Glimpse.WebApi.Inspector;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;
using Glimpse.Test.Common;
using Moq;
using Xunit;
using Xunit.Extensions;
using IHttpRoute = System.Web.Http.Routing.IHttpRoute;

namespace Glimpse.Test.WebApi.Inspector
{
    public class RoutesInspectorShould
    {
        [Fact]
        public void Construct()
        {
            var sut = new RoutesInspector();

            Assert.NotNull(sut);
            Assert.IsAssignableFrom<IInspector>(sut);
        }
         
        [Theory, AutoMock]
        public void IntergrationTestRouteProxing(RoutesInspector sut, System.Web.Http.Routing.IRouteHandler routeHandler, IInspectorContext context)
        {
            GlobalConfiguration.Configuration.Routes.Clear();
            GlobalConfiguration.Configuration.Routes.Add("Test", new HttpRoute("Test", routeHandler));
            GlobalConfiguration.Configuration.Routes.Add("BaseTyped", new NewIHttpRoute());
            GlobalConfiguration.Configuration.Routes.Add("BaseTestTyped", new NewConstructorIHttpRoute("Name"));
            GlobalConfiguration.Configuration.Routes.Add("SubTyped", new NewHttpRoute("test", routeHandler));
            GlobalConfiguration.Configuration.Routes.Add("SubTestTyped", new NewConstructorHttpRoute("test", routeHandler, "Name"));
            GlobalConfiguration.Configuration.Routes.Ignore("{resource}.axd/{*pathInfo}", new { resource = "Test", pathInfo = "[0-9]" });

            context.Setup(x => x.ProxyFactory).Returns(new CastleDynamicProxyFactory(context.Logger, context.MessageBroker, () => new ExecutionTimer(new Stopwatch()), () => new RuntimePolicy()));

            sut.Setup(context);

            // This test needs to be like this because IProxyTargetAccessor is in Moq and Glimpse
            foreach (var route in GlobalConfiguration.Configuration.Routes)
            {
                var found = false;
                foreach (var routeInterface in route.GetType().GetInterfaces())
                {
                    if (routeInterface.Name == "IProxyTargetAccessor")
                    {
                        found = true;
                    }
                }

                Assert.True(found);
            }
        }

        [Theory, AutoMock]
        public void ExtendsMvcRoutes(System.Web.Http.Routing.IRouteHandler routeHandler, RoutesInspector sut, IInspectorContext context, HttpRoute newRoute)
        {
            GlobalConfiguration.Configuration.Routes.Clear();
            GlobalConfiguration.Configuration.Routes.Add("Test", new Route("Test", routeHandler));

            context.ProxyFactory.Setup(x => x.ExtendClass<HttpRoute>(It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(newRoute, GlobalConfiguration.Configuration.Routes[0]);
        }

        [Theory, AutoMock]
        public void WrapsMvcRouteDerivedTypes(RoutesInspector sut, System.Web.Http.Routing.IRouteHandler routeHandler, IInspectorContext context, NewRoute route, HttpRoute newRoute)
        {
            GlobalConfiguration.Configuration.Routes.Clear();
            GlobalConfiguration.Configuration.Routes.Add("Test", route);

            context.ProxyFactory.Setup(x => x.IsWrapClassEligible(typeof(HttpRoute))).Returns(true).Verifiable();
            context.ProxyFactory.Setup(x => x.WrapClass((HttpRoute)route, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(newRoute, GlobalConfiguration.Configuration.Routes[0]);
        }

        [Theory, AutoMock]
        public void WrapsMvcIHttpRouteDerivedTypes(RoutesInspector sut, System.Web.Http.Routing.IRouteHandler routeHandler, IInspectorContext context, NewIHttpRoute route, IHttpRoute newRoute)
        {
            GlobalConfiguration.Configuration.Routes.Clear();
            GlobalConfiguration.Configuration.Routes.Add("Test", route);

            context.ProxyFactory.Setup(x => x.IsWrapClassEligible(typeof(IHttpRoute))).Returns(true);
            context.ProxyFactory.Setup(x => x.WrapClass((IHttpRoute)route, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(newRoute, GlobalConfiguration.Configuration.Routes[0]);
        }

        [Theory, AutoMock]
        public void ExtendsStringConstraints(RoutesInspector sut, IInspectorContext context, NewRoute route, HttpRoute newRoute, string routeConstraint)
        {
            route.Constraints = new RouteValueDictionary { { "controller", routeConstraint } };

            GlobalConfiguration.Configuration.Routes.Clear();
            GlobalConfiguration.Configuration.Routes.Add("Test", route);

            context.ProxyFactory.Setup(x => x.IsWrapClassEligible(typeof(HttpRoute))).Returns(true).Verifiable();
            context.ProxyFactory.Setup(x => x.WrapClass((HttpRoute)route, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(typeof(IHttpRouteConstraintRegex), route.Constraints["controller"].GetType());
        }
         
        [Theory, AutoMock]
        public void ExtendsRouteConstraintConstraints(RoutesInspector sut, IInspectorContext context, NewRoute route, HttpRoute newRoute, IHttpRouteConstraint routeConstraint, IHttpRouteConstraint newRouteConstraint)
        {
            route.Constraints = new RouteValueDictionary { { "controller", routeConstraint } };

            GlobalConfiguration.Configuration.Routes.Clear();
            GlobalConfiguration.Configuration.Routes.Add("Test", route);

            context.ProxyFactory.Setup(x => x.IsWrapClassEligible(typeof(HttpRoute))).Returns(true).Verifiable();
            context.ProxyFactory.Setup(x => x.WrapClass((HttpRoute)route, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();
            context.ProxyFactory.Setup(x => x.IsWrapInterfaceEligible<IHttpRouteConstraint>(typeof(IHttpRouteConstraint))).Returns(true).Verifiable();
            context.ProxyFactory.Setup(x => x.WrapInterface(routeConstraint, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>())).Returns(newRouteConstraint).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(newRouteConstraint, route.Constraints["controller"]);
        }


        public class NewIHttpRoute : IHttpRoute
        {
            public override System.Web.Http.Routing.RouteData GetRouteData(HttpContextBase httpContext)
            { 
                return new System.Web.Http.Routing.RouteData();
            }

            public override System.Web.Http.Routing.VirtualPathData GetVirtualPath(System.Web.Http.Routing.RequestContext requestContext, System.Web.Http.Routing.RouteValueDictionary values)
            {
                return new System.Web.Http.Routing.VirtualPathData(this, "Test");
            }
        }

        public class NewConstructorIHttpRoute : NewIHttpRoute
        {
            public NewConstructorIHttpRoute(string name)
            {
            }
        }

        public class NewRoute : HttpRoute
        {
            public NewRoute(string url, System.Web.Http.Routing.IRouteHandler routeHandler)
                : base(url, routeHandler)
            {
            }

            public NewRoute(string url, System.Web.Http.Routing.RouteValueDictionary defaults, System.Web.Http.Routing.IRouteHandler routeHandler)
                : base(url, defaults, routeHandler)
            {
            }

            public NewRoute(string url, System.Web.Http.Routing.RouteValueDictionary defaults, System.Web.Http.Routing.RouteValueDictionary constraints, System.Web.Http.Routing.IRouteHandler routeHandler)
                : base(url, defaults, constraints, routeHandler)
            {
            }

            public NewRoute(string url, System.Web.Http.Routing.RouteValueDictionary defaults, System.Web.Http.Routing.RouteValueDictionary constraints, System.Web.Http.Routing.RouteValueDictionary dataTokens, System.Web.Http.Routing.IRouteHandler routeHandler)
                : base(url, defaults, constraints, dataTokens, routeHandler)
            {
            }
        }

        public class NewConstructorRoute : HttpRoute
        {
            public NewConstructorRoute(string url, System.Web.Http.Routing.IRouteHandler routeHandler, string name)
                : base(url, routeHandler)
            {
            }
        }
    }
}
