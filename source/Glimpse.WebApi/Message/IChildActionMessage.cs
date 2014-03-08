using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using Glimpse.Core.Message;

namespace Glimpse.WebApi.Message
{
    public interface IChildActionMessage : IActionMessage
    {
        bool IsChildAction { get; set; }
    }

    public static class ChildActionMessageExtension
    {
        //public static T AsChildActionMessage<T>(this T message, HttpControllerContext controllerContext)
        //    where T : IChildActionMessage
        //{
        //    message.IsChildAction = controllerContext != null && controllerContext.Controller.IsChildAction;

        //    return message;
        //}

        //public static T AsChildActionMessage<T>(this T message, IHttpController httpController)
        //    where T : IChildActionMessage
        //{
        //    var controller = httpController as ApiController;
        //    if (controller != null)
        //    {
        //        message.AsChildActionMessage(controller.ControllerContext);
        //    }

        //    return message;
        //} 
    }
}
