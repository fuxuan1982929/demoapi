namespace demoapi.MQ;
public class Message
{
    public string GId { get; set; } = "";

    public string Channel { get; set; } = "";

    public string? Catalog { get; set; }

    public string From { get; set; } = "";

    public string Subject { get; set; } = "";

    public string Body { get; set; } = "";

}