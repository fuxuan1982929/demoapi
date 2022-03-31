using demoapi.Infrastructure.Idempotency;
using MediatR;

namespace demoapi.Application.Commands;

/// <summary>
/// Provides a base implementation for handling duplicate request and ensuring idempotent updates, in the cases where
/// a requestid sent by client is used to detect duplicate requests.
/// </summary>
/// <typeparam name="T">Type of the command handler that performs the operation if request is not duplicated</typeparam>
/// <typeparam name="R">Return value of the inner command handler</typeparam>
public class IdentifiedCommandHandler<T, R> : IRequestHandler<IdentifiedCommand<T, R>, R>
    where T : IRequest<R>
{
    private readonly IMediator _mediator;
    private readonly IRequestManager _requestManager; //幂等性
    private readonly ILogger<IdentifiedCommandHandler<T, R>> _logger;

    public IdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<T, R>> logger)
    {
        _mediator = mediator;
        _requestManager = requestManager;
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates the result value to return if a previous request was found
    /// </summary>
    /// <returns></returns>
    protected virtual R? CreateResultForDuplicateRequest()
    {
        return default(R);
    }

    /// <summary>
    /// This method handles the command. It just ensures that no other request exists with the same ID, and if this is the case
    /// just enqueues the original inner command.
    /// </summary>
    /// <param name="message">IdentifiedCommand which contains both original command & request ID</param>
    /// <returns>Return value of inner command or default value if request same ID was found</returns>
#pragma warning disable CS8613 // 返回类型中引用类型的为 Null 性与隐式实现的成员不匹配。
    public async Task<R?> Handle(IdentifiedCommand<T, R> message, CancellationToken cancellationToken)
#pragma warning restore CS8613 // 返回类型中引用类型的为 Null 性与隐式实现的成员不匹配。
    {
        var alreadyExists = await _requestManager.ExistAsync(message.Id);

        if (alreadyExists) //客户端重复调用
        {
#pragma warning disable CS8603 // 可能返回 null 引用。
            return CreateResultForDuplicateRequest();
#pragma warning restore CS8603 // 可能返回 null 引用。
        }
        else
        {
            //消息ID写入记录表
            await _requestManager.CreateRequestForCommandAsync<T>(message.Id);

            try
            {
                var command = message.Command;
                //var commandName = command.GetGenericTypeName();
                var commandName = "";
                var idProperty = string.Empty;
                var commandId = string.Empty;

                switch (command)
                {
                    case SendMQCommand sendMQCommand:
                        //idProperty = nameof(sendMQCommand.UserId); //命令关键字名称
                        //commandId = sendMQCommand.id; //命令关键字值
                        break;
                    default:
                        idProperty = "Id?";
                        commandId = "n/a";
                        break;
                }

                _logger.LogInformation(
                   "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                   commandName,
                   idProperty,
                   commandId,
                   command);

                var result = await _mediator.Send(command, cancellationToken);

                _logger.LogInformation(
                    "----- Command result: {@Result} - {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                    result,
                    commandName,
                    idProperty,
                    commandId,
                    command);

                return result;
            }
            catch
            {
                return default(R);
            }
        }
        

       
    }
}
