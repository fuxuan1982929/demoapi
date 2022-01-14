namespace demoapi.Webbase;

public class MyApiContext
{
    public static MyDataResult<string> GetClientToken(HttpContext httpContext)
    {
        return new MyDataResult<string>
        {
            Result = true,
            Message = "Sucess",
            Data = ""
        };
    }
}