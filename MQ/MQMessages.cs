using System.Text.Json;

namespace demoapi.MQ;

public class MQMessages
{
    public static Message AddLogMessage<T>(T model, string id, string opType, string opUser, string from)
        {
            var message = new Message();
            dynamic body = new System.Dynamic.ExpandoObject();
            body.OpUser = opUser;
            body.opType = opType;
            body.ModelId = id;
            body.Model = model;

            message.GId = Guid.NewGuid().ToString();
            message.Channel = "DataLog";
            message.Catalog = typeof(T).FullName;
            message.Body = JsonSerializer.Serialize(body);
            message.Subject = string.Format("{0} - {1} - {2}", message.Channel, message.Catalog, opType);
            message.From = from;
            return message;
        }

}