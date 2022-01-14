using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace demoapi.MQ;

public class RabbitMQHelper
{
    ConnectionFactory connectionFactory;
    IConnection connection;
    IModel channel;
    string exchangeName;

    private readonly IOptions<RabbitMQConfig> _rabbitMQConfig;

    public RabbitMQHelper(IOptions<RabbitMQConfig> rabbitMQConfig, string changeName = "fanout_mq")
    {
        _rabbitMQConfig = rabbitMQConfig;

        this.exchangeName = _rabbitMQConfig.Value.ExchangeName;
        //创建连接工厂
        connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMQConfig.Value.Host,
            Port = _rabbitMQConfig.Value.Port,
            UserName = _rabbitMQConfig.Value.UserName,
            Password = _rabbitMQConfig.Value.Password
        };
        //创建连接
        connection = connectionFactory.CreateConnection();
        //创建通道
        channel = connection.CreateModel();
        //声明交换机
        channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, true);

    }

    public void SendMsg<T>(string queName, T msg) where T : class
    {
        //声明一个队列
        channel.QueueDeclare(queName, true, false, false, null);
        //绑定队列，交换机，路由键
        channel.QueueBind(queName, exchangeName, queName);

        //开启confirm模式
        channel.ConfirmSelect();

        var basicProperties = channel.CreateBasicProperties();
        //1：非持久化 2：可持久化
        basicProperties.DeliveryMode = 2;
        basicProperties.Persistent = true;

        //var payload = Encoding.UTF8.GetBytes("我发出的消息");
        var address = new PublicationAddress(ExchangeType.Direct, exchangeName, queName);
        //channel.BasicPublish(address, basicProperties, payload);

        var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg));
        channel.BasicPublish(address, basicProperties, payload);


        //等待批量发送成功的确认
        if (channel.WaitForConfirms())
        {
            Console.WriteLine("发送成功");
        }
        else
        {
            Console.WriteLine("发送失败:" + JsonSerializer.Serialize(msg));
        }

    }

    public bool SendMQ<T>(string queName, T msg) where T : class
    {
        var _rst = false;

        //声明一个队列
        channel.QueueDeclare(queName, true, false, false, null);
        //绑定队列，交换机，路由键
        channel.QueueBind(queName, exchangeName, queName);

        //开启confirm模式
        channel.ConfirmSelect();

        var basicProperties = channel.CreateBasicProperties();
        //1：非持久化 2：可持久化
        basicProperties.DeliveryMode = 2;
        basicProperties.Persistent = true;

        var address = new PublicationAddress(ExchangeType.Direct, exchangeName, queName);
        var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg));
        channel.BasicPublish(address, basicProperties, payload);


        //等待批量发送成功的确认
        if (channel.WaitForConfirms())
        {
            //Console.WriteLine("发送成功");
            _rst = true;
        }
        else
        {
            _rst = false;
            //Console.WriteLine("发送失败:" + JsonSerializer.Serialize(msg));
        }

        return _rst;

    }
}