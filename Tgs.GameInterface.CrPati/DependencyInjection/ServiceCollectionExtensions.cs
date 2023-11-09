using Tgs.GameInterface.CrPati.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameInterface(
        this IServiceCollection services,
        Action<GameApiSettingsOptions, IServiceProvider> gameApiSettingsOptions,
        Action<PolicySettingsOptions, IServiceProvider> policySettingsOptions)
        => services
        .AddGameInterfaceCore(
            gameApiSettingsOptions,
            policySettingsOptions);
}
