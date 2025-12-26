namespace CineVault.API.Extensions;

public static class IHostEnvironmentExtensions
{
    public static bool IsLocal(this IHostEnvironment env)
    {
        // Перевірка на співпадіння середовища з "Local" без урахування регістру
        return env.EnvironmentName.Equals("Local", StringComparison.OrdinalIgnoreCase);
    }
}

