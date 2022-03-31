namespace demoapi.Webbase;

public class MyApiContext
{
    public static MyDataResult<string> GetClientToken(HttpContext httpContext)
    {
        //判断httpContext口令是否验证通过
        var result = new MyDataResult<string>();
        //TODO
        string tokenValue = "AA"; // GetSSOToken(httpContext);

        if (string.IsNullOrWhiteSpace(tokenValue))
        {
            result.Result = false;
            result.Message = "用户凭据不存在！";
            result.Data = string.Empty;
            return result;
        }

        try
        {
            //TODO
            var uname = ""; // GetUName(tokenValue);
            return new MyDataResult<string>
            {
                Result = true,
                Message = "Sucess",
                Data = uname
            };
        }
        catch (Exception ex)
        {
            result.Result = false;
            result.Message = ex.Message;
            result.Data = string.Empty;
        }

        return result;
      
    }
}