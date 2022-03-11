using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
        channel.QueueDeclare(queue: queName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

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

        channel.BasicPublish(addr: address,
                             basicProperties: basicProperties,
                             body: payload);


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

    public void Receive(string queueName, Action<string> received)
    {
        //事件基本消费者
        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

        //接收到消息事件
        consumer.Received += (ch, ea) =>
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());
            received(message);
            //确认该消息已经被消费
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        //启动消费者， 设置手动应答消息
        channel.BasicConsume(queueName, false, consumer);
    }

    //消费消息，判断成功才删除队列
    public void ReceiveNew(string queueName, Func<string, bool> received)
    {
        //事件基本消费者
        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

        //接收到消息事件
        consumer.Received += (ch, ea) =>
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var result = received(message);
            if (result)
            {
                //确认该消息已经被消费
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };
        //启动消费者， 设置手动应答消息
        channel.BasicConsume(queueName, false, consumer);
    }

    public void ReceiveAck(string queueName, Func<string, bool> received)
    {
        //事件基本消费者
        EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
        //限流一次获取最大未确认消息数量 10条
        channel.BasicQos(0, 10, false);
        //接收到消息事件
        consumer.Received += (ch, ea) =>
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var result = received(message);
            if (result)
            {
                //确认该消息已经被消费
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            else if(!result)
            {
                //方案1： 使用BasicNack， 将消息重新放回队列重新消费。
                channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };
        //启动消费者， 设置手动应答消息
        channel.BasicConsume(queueName, false, consumer);
    }
}