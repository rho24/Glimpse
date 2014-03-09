using System.Web.Http;
using Glimpse.Core.Extensibility;

namespace Glimpse.WebApi.Inspector
{
    public class ExecutionInspector : IInspector
    {
        public void Setup(IInspectorContext context)
        {
            var logger = context.Logger;

            //GlobalConfiguration.Configuration.MessageHandlers.Add(new GlimpseHandler());

            var dummyAuthorizationFilter = new DummyAuthorizationFilter(logger);
            var alternateAuthorizationFilterImplementation = new Glimpse.WebApi.AlternateType.AuthorizationFilterAttribute(context.ProxyFactory);
            System.Web.Http.Filters.AuthorizationFilterAttribute newAuthorizationFilter;

            if (alternateAuthorizationFilterImplementation.TryCreate(dummyAuthorizationFilter, out newAuthorizationFilter))
            {
                GlobalConfiguration.Configuration.Filters.Add(newAuthorizationFilter);
                logger.Debug(Resources.ActionFilterSetup, dummyAuthorizationFilter.GetType());
            }

            var dummyActionFilter = new DummyActionFilter(logger);
            var alternateActionFilterImplementation = new Glimpse.WebApi.AlternateType.ActionFilterAttribute(context.ProxyFactory);
            System.Web.Http.Filters.ActionFilterAttribute newActionFilter;

            if (alternateActionFilterImplementation.TryCreate(dummyActionFilter, out newActionFilter))
            {
                GlobalConfiguration.Configuration.Filters.Add(newActionFilter);
                logger.Debug(Resources.ActionFilterSetup, dummyActionFilter.GetType());
            }
        }
    }
}