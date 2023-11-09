using System.Reflection;
using Tgs.GameInterface.CrPati.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameInterfaceCore(
        this IServiceCollection services,
        Action<GameApiSettingsOptions, IServiceProvider> gameApiSettingsOptions,
        Action<PolicySettingsOptions, IServiceProvider> policySettingsOptions)
        => services
        .AddOptions<GameApiSettingsOptions>()
        .Configure(gameApiSettingsOptions)
        .Services
        .AddOptions<PolicySettingsOptions>()
        .Configure(policySettingsOptions)
        .Services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddHttpClient<IGameApiClient, GameApiClient>()
        .Services
        .AddSingleton<IHashCryptography, HashCryptography>()
        .AddSingleton<ISystemClock, SystemClock>();
}
