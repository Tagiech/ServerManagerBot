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

    public async Task<UserResponse> DispatchExclusive(CommandDescriptor descriptor, CommandContext context, CancellationToken ct)
    {
        var gate = _userGateRegistry.Get(context.UserId);
        await gate.WaitAsync(ct);
        try
        {
            return await Dispatch(descriptor, context, ct);
        }
        catch (Exception ex) //TODO: catch specific exceptions
        {
            return new TextResponse($"{ex.Message} {ex.StackTrace}");
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<UserResponse> Dispatch(CommandDescriptor descriptor, CommandContext context, CancellationToken ct)
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
            .MakeGenericMethod(commandType, typeof(UserResponse));

        return await (Task<UserResponse>)sendMethod.Invoke(
            _mediator,
            [request, ct])!;
    }
}