namespace ServerManagerBot.Application.Commands.UserCommands.Responses;

public abstract record UserResponse;

public sealed record TextResponse(string Text, bool Markdown = false) : UserResponse;

public sealed record ImageResponse(byte[] Image, string? Caption = null) : UserResponse;

public sealed record MultiResponse(IReadOnlyList<UserResponse> Items) : UserResponse;

public sealed record NoResponse : UserResponse;
