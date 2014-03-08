using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Glimpse.Core.Extensibility;
using Glimpse.WebApi.AlternateType;

namespace Glimpse.WebApi.Inspector
{
    public class ExecutionInspector : IInspector
    {
        public void Setup(IInspectorContext context)
        {
            var logger = context.Logger;

            //var originalControllerFactory = ControllerBuilder.Current.GetControllerFactory();
            //var alternateImplementation = new ControllerFactory(context.ProxyFactory);
            //IControllerFactory newControllerFactory;

            //if (alternateImplementation.TryCreate(originalControllerFactory, out newControllerFactory))
            //{
            //    ControllerBuilder.Current.SetControllerFactory(newControllerFactory);

            //    logger.Debug(Resources.ControllerFactorySetup, originalControllerFactory.GetType());
            //}

            //GlobalConfiguration.Configuration.Filters.Add(new AuditActionFilter());

            //System.Web.Mvc.GlobalFilters.Filters.Add(new AuditActionFilter());

            GlobalConfiguration.Configuration.MessageHandlers.Add(new InfoHandler());

            var dummyActionFilter = new GlimpseDummyActionFilter();
            var alternateImplementation = new ActionFilter(context.ProxyFactory);
            IActionFilter newActionFilter;

            if (alternateImplementation.TryCreate(dummyActionFilter, out newActionFilter))
            {
                GlobalConfiguration.Configuration.Filters.Add(newActionFilter);
                logger.Debug(Resources.ActionFilterSetup, dummyActionFilter.GetType());
            }


        }
    }

    public class InfoHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            //publish info from request and response somewhere
            return response;
        }
    }

    public class GlimpseDummyActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            Trace.TraceWarning("Glimpse Dummy Action Filter  - OnActionExecuting:- Controller:{0} Action:{1} Arguments:{2}",
               context.ControllerContext.ControllerDescriptor.ControllerName,
               context.ActionDescriptor.ActionName,
               context.ActionArguments.Count);
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            object returnVal = null;
            var oc = context.Response.Content as ObjectContent;
            if (oc != null)
                returnVal = oc.Value;

            Trace.TraceWarning("Glimpse Dummy Action Filter  - OnActionExecuted:- Controller:{0} Action:{1} Result:{2}",
               context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName,
               context.ActionContext.ActionDescriptor.ActionName,
               returnVal ?? "null");
        }
    }
}