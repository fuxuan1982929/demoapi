namespace demoapi.Webbase;

public class MyDataResult<T>
{

    public bool Result { get; set; }

    public int Code { get; set; }   

    public string Message { get; set; } = "";

    public T? Data { get; set;}
}