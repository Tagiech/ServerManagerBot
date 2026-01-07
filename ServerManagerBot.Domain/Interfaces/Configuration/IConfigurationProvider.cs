namespace ServerManagerBot.Domain.Interfaces.Configuration;

public interface IConfigurationProvider
{
    long[] GetAllowedUserIds();
}