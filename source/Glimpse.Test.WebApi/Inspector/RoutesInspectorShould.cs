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
        public void IntergrationTestRouteProxing(RoutesInspector sut, System.Net.Http.HttpMessageHandler routeHandler, IInspectorContext context)
        {
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Clear();
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("Test", new HttpRoute("Test", routeHandler));
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("BaseTyped", new NewIHttpRoute());
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("BaseTestTyped", new NewConstructorIHttpRoute("Name"));
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("SubTyped", new NewHttpRoute("test", routeHandler));
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("SubTestTyped", new NewConstructorHttpRoute("test", routeHandler, "Name"));
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Ignore("{resource}.axd/{*pathInfo}", new { resource = "Test", pathInfo = "[0-9]" });

            context.Setup(x => x.ProxyFactory).Returns(new CastleDynamicProxyFactory(context.Logger, context.MessageBroker, () => new ExecutionTimer(new Stopwatch()), () => new RuntimePolicy()));

            sut.Setup(context);

            // This test needs to be like this because IProxyTargetAccessor is in Moq and Glimpse
            foreach (var route in System.Web.Http.GlobalConfiguration.Configuration.Routes)
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
        public void ExtendsMvcRoutes(System.Net.Http.HttpMessageHandler routeHandler, RoutesInspector sut, IInspectorContext context, HttpRoute newRoute)
        {
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Clear();
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("Test", new HttpRoute("Test", routeHandler));

            context.ProxyFactory.Setup(x => x.ExtendClass<HttpRoute>(It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(newRoute, System.Web.Http.GlobalConfiguration.Configuration.Routes[0]);
        }

        [Theory, AutoMock]
        public void WrapsMvcRouteDerivedTypes(RoutesInspector sut, System.Net.Http.HttpMessageHandler routeHandler, IInspectorContext context, NewHttpRoute route, HttpRoute newRoute)
        {
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Clear();
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("Test", route);

            context.ProxyFactory.Setup(x => x.IsWrapClassEligible(typeof(HttpRoute))).Returns(true).Verifiable();
            context.ProxyFactory.Setup(x => x.WrapClass((HttpRoute)route, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(newRoute, System.Web.Http.GlobalConfiguration.Configuration.Routes[0]);
        }

        [Theory, AutoMock]
        public void WrapsMvcIHttpRouteDerivedTypes(RoutesInspector sut, System.Net.Http.HttpMessageHandler routeHandler, IInspectorContext context, NewIHttpRoute route, IHttpRoute newRoute)
        {
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Clear();
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("Test", route);

            context.ProxyFactory.Setup(x => x.IsWrapClassEligible(typeof(IHttpRoute))).Returns(true);
            context.ProxyFactory.Setup(x => x.WrapClass((IHttpRoute)route, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(newRoute, System.Web.Http.GlobalConfiguration.Configuration.Routes[0]);
        }

        [Theory, AutoMock]
        public void ExtendsStringConstraints(RoutesInspector sut, IInspectorContext context, NewHttpRoute route, HttpRoute newRoute, string routeConstraint)
        {
            route.Constraints = new HttpRouteValueDictionary { { "controller", routeConstraint } };

            System.Web.Http.GlobalConfiguration.Configuration.Routes.Clear();
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("Test", route);

            context.ProxyFactory.Setup(x => x.IsWrapClassEligible(typeof(HttpRoute))).Returns(true).Verifiable();
            context.ProxyFactory.Setup(x => x.WrapClass((HttpRoute)route, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(typeof(IHttpRouteConstraintRegex), route.Constraints["controller"].GetType());
        }
         
        [Theory, AutoMock]
        public void ExtendsRouteConstraintConstraints(RoutesInspector sut, IInspectorContext context, NewHttpRoute route, HttpRoute newRoute, System.Web.Http.Routing.IHttpRouteConstraint routeConstraint, System.Web.Http.Routing.IHttpRouteConstraint newRouteConstraint)
        {
            route.Constraints = new HttpRouteValueDictionary { { "controller", routeConstraint } };

            System.Web.Http.GlobalConfiguration.Configuration.Routes.Clear();
            System.Web.Http.GlobalConfiguration.Configuration.Routes.Add("Test", route);

            context.ProxyFactory.Setup(x => x.IsWrapClassEligible(typeof(HttpRoute))).Returns(true).Verifiable();
            context.ProxyFactory.Setup(x => x.WrapClass((HttpRoute)route, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>(), It.IsAny<object[]>())).Returns(newRoute).Verifiable();
            context.ProxyFactory.Setup(x => x.IsWrapInterfaceEligible<System.Web.Http.Routing.IHttpRouteConstraint>(typeof(System.Web.Http.Routing.IHttpRouteConstraint))).Returns(true).Verifiable();
            context.ProxyFactory.Setup(x => x.WrapInterface(routeConstraint, It.IsAny<IEnumerable<IAlternateMethod>>(), It.IsAny<IEnumerable<object>>())).Returns(newRouteConstraint).Verifiable();

            sut.Setup(context);

            context.ProxyFactory.VerifyAll();
            Assert.Same(newRouteConstraint, route.Constraints["controller"]);
        }


        public class NewIHttpRoute : IHttpRoute
        {
            public System.Web.Http.Routing.IHttpRouteData GetRouteData(string virtualPathRoot, System.Net.Http.HttpRequestMessage request)
            { 
                return new System.Web.Http.Routing.HttpRouteData();
            }

            public System.Web.Http.Routing.IHttpVirtualPathData GetVirtualPath(System.Net.Http.HttpRequestMessage request, IDictionary<string, object> values)
            {
                return new System.Web.Http.Routing.HttpVirtualPathData(this, "Test");
            }
          
            public string RouteTemplate {
              get {
                throw new NotImplementedException();
              }
            }
                  
            public IDictionary<string, object> Defaults {
              get {
                return new System.Web.Http.Routing.HttpRouteValueDictionary { { "Test", "Other" } };
              }
            }
                  
            public IDictionary<string, object> Constraints {
              get {
                throw new NotImplementedException();
              }
            }
                  
            public IDictionary<string, object> DataTokens {
              get {
                return new System.Web.Http.Routing.HttpRouteValueDictionary { { "Data", "Tokens" } };
              }
            }
                  
            public System.Net.Http.HttpMessageHandler Handler {
              get {
                throw new NotImplementedException();
              }
            }
        }

        public class NewConstructorIHttpRoute : NewIHttpRoute
        {
            public NewConstructorIHttpRoute(string name)
            {
            }
        }

        public class NewHttpRoute : HttpRoute
        {
            public NewHttpRoute()
                : base()
            {
            }
            
            public NewHttpRoute(string url)
                : base(url)
            {
            }
            
            public NewHttpRoute(string url, System.Web.Http.Routing.HttpRouteValueDictionary defaults)
                : base(url, defaults)
            {
            }

            public NewHttpRoute(string url, System.Web.Http.Routing.HttpRouteValueDictionary defaults, System.Web.Http.Routing.HttpRouteValueDictionary constraints)
                : base(url, defaults, constraints)
            {
            }

            public NewHttpRoute(string url, System.Web.Http.Routing.HttpRouteValueDictionary defaults, System.Web.Http.Routing.HttpRouteValueDictionary constraints, System.Web.Http.Routing.HttpRouteValueDictionary dataTokens)
                : base(url, defaults, constraints, dataTokens)
            {
            }

            public NewHttpRoute(string url, System.Web.Http.Routing.HttpRouteValueDictionary defaults, System.Web.Http.Routing.HttpRouteValueDictionary constraints, System.Web.Http.Routing.HttpRouteValueDictionary dataTokens, System.Net.Http.HttpMessageHandler routeHandler)
                : base(url, defaults, constraints, dataTokens, routeHandler)
            {
            }
        }

        public class NewConstructorHttpRoute : HttpRoute
        {
            public NewConstructorHttpRoute(string url, System.Net.Http.HttpMessageHandler routeHandler, string name)
                : base(url, routeHandler)
            {
            }
        }
    }
}
