using demoapi.Infrastructure.Idempotency;
using demoapi.MQ;
using MediatR;

namespace demoapi.Application.Commands;

public class SendMQCommandHander : IRequestHandler<SendMQCommand, bool>
{
    private readonly RabbitMQHelper _mQHelpler;
    public SendMQCommandHander(RabbitMQHelper mQHelpler)
    {
        _mQHelpler = mQHelpler;

    }

    public async Task<bool> Handle(SendMQCommand request, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            string msg = request.Msg;
            return _mQHelpler.SendMQ<string>("queue1", msg);
        });
    }
}

// Use for Idempotency in Command process
// NOTE: IdentifiedCommandHandler 继承类必须填写，否则映射失败
public class SendMQCommandIdentifiedCommandHandler : IdentifiedCommandHandler<SendMQCommand, bool>
{
    public SendMQCommandIdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<SendMQCommand, bool>> logger)
        : base(mediator, requestManager, logger)
    {
    }

    protected override bool CreateResultForDuplicateRequest()
    {
        return true; // Ignore duplicate requests for processing order.
    }
}