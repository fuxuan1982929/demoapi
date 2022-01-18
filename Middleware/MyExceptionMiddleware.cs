namespace demoapi.Middleware;

public class MyExceptionMiddleware
{
    private readonly RequestDelegate next;

    public MyExceptionMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
            //var features = context.Features;
        }
        catch (Exception e)
        {
            await HandleException(context, e);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/json;charset=utf-8;";
        // if (ex is Validate.UIValidateException)
        // {
        //     // nothing to do
        // }
        // else
        // {}

        // 写错误日志到控制台
        Console.WriteLine(ex.ToString());
        // 返回错误信息
        await OutputResult(context, ex.Message);
    }

    private async Task OutputResult(HttpContext context, string message)
    {
        var result = new Webbase.MyDataResult<dynamic>
        {
            Result = false,
            Code = 500,
            Message = message,
            Data = null
        };

        await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(result));
    }
}