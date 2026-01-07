using JetBrains.Annotations;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Application.Commands.UserCommands.Responses;
using ServerManagerBot.Domain.Interfaces.Media;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Application.Commands.UserCommands.Requests;

[UsedImplicitly]
[UserCommand("movie",
    CommandSource.Text | CommandSource.InlineQuery,
    "Fetch information about a movie",
    "фильм, кино")]
public class SearchMovieRequest : IParsableRequest<SearchMovieRequest, CommandResponse>
{
    public string SourceId { get; }
    public CommandSource Source { get; }
    public string Query { get; }

    public SearchMovieRequest(string sourceId, CommandSource source, string query)
    {
        SourceId = sourceId;
        Source = source;
        Query = query;
    }

    public static SearchMovieRequest Parse(CommandContext context)
        => new(context.SourceId, context.Source, context.Query);
}

[UsedImplicitly]
public class SearchMovieRequestHandler : IRequestHandler<SearchMovieRequest, CommandResponse>
{
    private readonly IMediaSearchService _mediaSearchService;

    public SearchMovieRequestHandler(IMediaSearchService mediaSearchService)
    {
        _mediaSearchService = mediaSearchService;
    }

    public async Task<CommandResponse> Handle(SearchMovieRequest request, CancellationToken ct)
    {
        var response = new CommandResponse(request.SourceId);
        if ((request.Source & CommandSource.Text) != 0)
            return response
                .WithText($"""
                           You want to search for a "{request.Query}"?
                           """)
                .WithInlineButtons([
                    [
                        new InlineButton { Caption = "Yes", InlineQuery = $"movie {request.Query}" },
                        new InlineButton { Caption = "No", CallbackData = "cancel" }
                    ]
                ]);

        var searchResults = await _mediaSearchService.SearchAsync(request.Query, ct);
        if (searchResults.Length == 0)
        {
            return response.WithText("No movies found");
        }

        var thumbnails = searchResults
            .Select(i =>
            {
                var (title, description) = FormatMediaInfo(i.Title, i.Type);
                return new ImageData(i.Id, i.ThumbnailUri, title, description);
            })
            .ToArray();

        return response.WithInlineImages(thumbnails);
    }

    private static (string title, string description) FormatMediaInfo(string title, string type)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return (string.Empty, string.Empty);
        }

        var raw = title.Trim();
        var parts = raw.Split(" / ", 2, StringSplitOptions.TrimEntries);

        var ruTitle = parts[0];
        var right = parts.Length > 1 ? parts[1] : string.Empty;

        int? year = null;
        string? enTitle = null;

        var openParen = right.LastIndexOf('(');
        if (openParen >= 0 && right.EndsWith(')'))
        {
            var yearText = right[(openParen + 1)..^1];
            if (int.TryParse(yearText, out var y))
            {
                year = y;
                enTitle = right[..openParen].Trim();
            }
        }

        var descriptionParts = new[] { type, year?.ToString(), enTitle }
            .Where(p => !string.IsNullOrWhiteSpace(p));

        return (ruTitle, string.Join(" • ", descriptionParts));
    }
}