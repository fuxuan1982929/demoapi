namespace demoapi.MQ;
public class RabbitMQConfig
{
    public int Port { get; set; }
    public string Host { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";

    public string ExchangeName { get; set; } = "";
    public string ExchhangeType { get; set; } = "";
    public string QueueName { get; set; } = "";
    public string RouteKey { get; set; } = "";
}