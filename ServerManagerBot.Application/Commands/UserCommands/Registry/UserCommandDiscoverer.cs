using System.Reflection;

namespace ServerManagerBot.Application.Commands.UserCommands.Registry;

public class UserCommandDiscoverer
{
    public static IReadOnlyCollection<CommandDescriptor> Discover(
        params Assembly[] assemblies)
    {
        var result = new List<CommandDescriptor>();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    t.GetCustomAttribute<UserCommandAttribute>() is not null &&
                    t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IParsableRequest<,>)));

            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<UserCommandAttribute>()!;

                result.Add(new CommandDescriptor(attribute.Name,
                    attribute.Description,
                    attribute.Aliases,
                    type));
            }
        }

        return result;
    }
}