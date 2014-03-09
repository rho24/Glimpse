using System.Collections.Generic;
using System.Web.Http.Controllers;
using Glimpse.Core.Extensibility;

namespace Glimpse.WebApi.AlternateType
{
    public class IHttpActionInvoker : AlternateType<System.Web.Http.Controllers.IHttpActionInvoker>
    {
        private IEnumerable<IAlternateMethod> allMethods;

        public IHttpActionInvoker(IProxyFactory proxyFactory)
            : base(proxyFactory)
        {
        }

        public override IEnumerable<IAlternateMethod> AllMethods
        {
            get
            {
                return allMethods ?? (allMethods = new List<IAlternateMethod>
                {
                    new InvokeActionAsync(),
                    new InvokeActionAsyncCore()
                });
            }
        }

        public class InvokeActionAsync : AlternateMethod
        {
            public InvokeActionAsync()
                : base(typeof(System.Web.Http.Controllers.IHttpActionInvoker), "InvokeActionAsync")
            {
            }

            public override void PostImplementation(IAlternateMethodContext context, TimerResult timerResult)
            {
                var actionContext = (HttpActionContext)context.Arguments[0];
            }
        }

        public class InvokeActionAsyncCore : AlternateMethod
        {
            public InvokeActionAsyncCore()
                : base(typeof(System.Web.Http.Controllers.IHttpActionInvoker), "InvokeActionAsyncCore")
            {
            }

            public override void PostImplementation(IAlternateMethodContext context, TimerResult timerResult)
            {
                var actionContext = (HttpActionContext)context.Arguments[0];
            }
        }
    }
}
