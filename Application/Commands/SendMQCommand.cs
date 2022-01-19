using System.Runtime.Serialization;
using MediatR;

namespace demoapi.Application.Commands;

public class SendMQCommand : IRequest<bool>
{
    [DataMember]
    public string Msg { get; private set; }

    public SendMQCommand(string msg)
    {
        Msg = msg;
    }
}