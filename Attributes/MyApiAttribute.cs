using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using demoapi.Webbase;

namespace demoapi.Attributes;
public class MyApi : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        //AllowAnonymous直接跳过
        if (context.Filters.All(item => item is IAllowAnonymousFilter)) return;

        var endpoint = context.HttpContext.Features.Get<IEndpointFeature>()?.Endpoint;

        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null) return;

        if (!(context.ActionDescriptor is ControllerActionDescriptor)) return;

        var tokenResult = MyApiContext.GetClientToken(context.HttpContext);
        if (!tokenResult.Result)
        {
            OutputResult(context, tokenResult.Message);
        }
    }

    private void OutputResult(AuthorizationFilterContext filterContext, string message)
    {
        var result = new Webbase.MyDataResult<dynamic>
        {
            Result = false,
            Code = 401,
            Message = message,
            Data = string.Empty
        };

        var actionResult = new ContentResult
        {
            Content = Newtonsoft.Json.JsonConvert.SerializeObject(result)
        };

        filterContext.Result = actionResult;
    }

}