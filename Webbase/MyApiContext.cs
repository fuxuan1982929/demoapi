namespace demoapi.Webbase;

public class MyApiContext
{
    public static MyDataResult<string> GetClientToken(HttpContext httpContext)
    {
        //�ж�httpContext�����Ƿ���֤ͨ��
        var result = new MyDataResult<string>();
        //TODO
        string tokenValue = "AA"; // GetSSOToken(httpContext);

        if (string.IsNullOrWhiteSpace(tokenValue))
        {
            result.Result = false;
            result.Message = "�û�ƾ�ݲ����ڣ�";
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