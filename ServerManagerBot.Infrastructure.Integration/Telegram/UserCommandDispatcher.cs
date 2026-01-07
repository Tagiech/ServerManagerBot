using System.Reflection;
using ServerManagerBot.Application.Commands;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Application.Commands.UserCommands.Responses;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Infrastructure.Integration.Telegram;

public sealed class UserCommandDispatcher
{
    private readonly IMediator _mediator;
    private readonly UserGateRegistry _userGateRegistry;

    public UserCommandDispatcher(IMediator mediator, UserGateRegistry userGateRegistry)
    {
        _mediator = mediator;
        _userGateRegistry = userGateRegistry;
    }

    public async Task<CommandResponse> DispatchExclusive(CommandDescriptor descriptor, CommandContext context,
        CancellationToken ct)
    {
        var gate = _userGateRegistry.Get(context.SourceId);
        await gate.WaitAsync(ct);
        try
        {
            return await Dispatch(descriptor, context, ct);
        }
        catch (Exception ex) //TODO: catch specific exceptions
        {
            return new CommandResponse(context.SourceId).WithText($"{ex.Message} {ex.StackTrace}");
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<CommandResponse> Dispatch(CommandDescriptor descriptor, CommandContext context,
        CancellationToken ct)
    {
        var commandType = descriptor.CommandType;

        var parseMethod = commandType.GetMethod(
                              "Parse",
                              BindingFlags.Public | BindingFlags.Static)
                          ?? throw new InvalidOperationException(
                              $"Command '{commandType.Name}' must define static Parse(CommandContext).");

        var request = parseMethod.Invoke(null, [context])!;

        var sendMethod = typeof(IMediator)
            .GetMethods()
            .Single(m =>
                m is { Name: nameof(IMediator.Send), IsGenericMethodDefinition: true } &&
                m.GetGenericArguments().Length == 2)
            .MakeGenericMethod(commandType, typeof(CommandResponse));

        return await (Task<CommandResponse>)sendMethod.Invoke(
            _mediator,
            [request, ct])!;
    }
}