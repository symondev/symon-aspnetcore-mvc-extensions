using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Symon.AspNetCore.Mvc.Extensions.Result
{
    public class ResultActionFilterAttributer : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var success = context.Exception == null;
            if (!success)
            {
                if (context.Exception is NonSystemException)
                {
                    var ex = (NonSystemException) context.Exception;
                    context.Result = new ObjectResult(new ApiResult(ex.ErrorCode, context.Exception.Message));
                    context.ExceptionHandled = true;
                }
            }
            else
            {
                if (context.Result != null)
                {
                    if (context.Result is ObjectResult)
                    {
                        var originalResult = (ObjectResult)context.Result;
                        context.Result = new ObjectResult(new ApiResult(originalResult.Value));
                    }
                    else if (context.Result is EmptyResult)
                    {
                        context.Result = new ObjectResult(new ApiResult());
                    }
                    else if (context.Result is JsonResult)
                    {
                        var originalResult = (JsonResult)context.Result;
                        context.Result = new JsonResult(new ApiResult(originalResult.Value));
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(context.Result));
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
