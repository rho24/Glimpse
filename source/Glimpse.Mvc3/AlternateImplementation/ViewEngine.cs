﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Glimpse.Core;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Message;

namespace Glimpse.Mvc.AlternateImplementation
{
    using Glimpse.Mvc.Message;

    public class ViewEngine : Alternate<IViewEngine>
    {
        public ViewEngine(IProxyFactory proxyFactory) : base(proxyFactory)
        {
        }

        public override IEnumerable<IAlternateMethod> AllMethods()
        {
            var alternateView = new View(ProxyFactory);
            yield return new FindViews(false, alternateView);
            yield return new FindViews(true, alternateView);
        }

        // This class is the alternate implementation for both .FindView() AND .FindPartialView()
        public class FindViews : AlternateMethod
        {
            public FindViews(bool isPartial, Alternate<IView> alternateView) : base(typeof(IViewEngine), isPartial ? "FindPartialView" : "FindView")
            {
                AlternateView = alternateView;
                IsPartial = isPartial;
            }

            public Alternate<IView> AlternateView { get; set; }

            public bool IsPartial { get; set; }

            public override void PostImplementation(IAlternateImplementationContext context, TimerResult timerResult)
            {
                var input = new Arguments(IsPartial, context.Arguments);
                var id = Guid.NewGuid();
                var output = context.ReturnValue as ViewEngineResult;

                output = ProxyOutput(output, context, input.ViewName, IsPartial, id);

                context.MessageBroker.PublishMany(
                    new Message(input, output, timerResult, context.TargetType, IsPartial, id),
                    new EventMessage(input, timerResult, context.TargetType, context.MethodInvocationTarget));
            }

            private ViewEngineResult ProxyOutput(ViewEngineResult viewEngineResult, IAlternateImplementationContext context, string viewName, bool isPartial, Guid id)
            {
                if (viewEngineResult.View != null)
                {
                    var originalView = viewEngineResult.View;

                    IView newView;
                    if (AlternateView.TryCreate(originalView, out newView, new[] { new ViewCorrelationMixin(viewName, isPartial, id) }))
                    {
                        context.Logger.Info(Resources.FindViewsProxyOutputReplacedIView, originalView.GetType(), viewName);

                        var result = new ViewEngineResult(newView, viewEngineResult.ViewEngine);
                        context.ReturnValue = result;
                        return result;
                    }
                }

                return viewEngineResult;
            }

            public class Message : MessageBase
            {
                public Message(Arguments input, ViewEngineResult output, TimerResult timing, Type baseType, bool isPartial, Guid id) : base(id)
                {
                    if (input == null)
                    {
                        throw new ArgumentNullException("input");
                    }

                    if (output == null)
                    {
                        throw new ArgumentNullException("output");
                    }

                    if (timing == null)
                    {
                        throw new ArgumentNullException("timing");
                    }

                    Input = input;
                    Output = output;
                    Timing = timing;

                    IsPartial = isPartial;
                    BaseType = baseType;
                }

                public Arguments Input { get; set; }
                
                public ViewEngineResult Output { get; set; }
                
                public TimerResult Timing { get; set; }
                
                public Type BaseType { get; set; }

                public bool IsPartial { get; set; }
                
                public bool IsFound
                {
                    get { return Output.View != null; }
                }
            }

            public class EventMessage : ActionMessage
            {
                public EventMessage(Arguments arguments, TimerResult timerResult, Type executedType, MethodInfo method)
                    : base(timerResult, GetControllerName(arguments.ControllerContext), GetActionName(arguments.ControllerContext), GetIsChildAction(arguments.ControllerContext), executedType, method)
                { 
                    ViewEngineType = executedType;
                    UseCache = arguments.UseCache;

                    EventName = string.Format("Find:View - {0}:{1}", ControllerName, ActionName);
                    EventSubText = string.Format("{0}:{1}", ViewEngineType.Name, UseCache);
                    EventCategory = "View";
                }
                 
                public Type ViewEngineType { get; private set; }

                public bool UseCache { get; private set; } 
            }

            public class Arguments
            {
                public Arguments(bool isPartial, params object[] arguments)
                {
                    ControllerContext = (ControllerContext)arguments[0];
                    ViewName = (string)arguments[1];
                    UseCache = isPartial ? (bool)arguments[2] : (bool)arguments[3];
                    MasterName = isPartial ? string.Empty : (string)arguments[2];
                }

                public ControllerContext ControllerContext { get; set; }
                
                public string ViewName { get; set; }
                
                public string MasterName { get; set; }
                
                public bool UseCache { get; set; }
            }
        }
    }
}