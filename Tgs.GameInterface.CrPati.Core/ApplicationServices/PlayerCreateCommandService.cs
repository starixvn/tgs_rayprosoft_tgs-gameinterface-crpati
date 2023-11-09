using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

internal class PlayerCreateCommandService : ICommandService<PlayerCreateCommand>
{
    private readonly IGameApiClient _client;

    public PlayerCreateCommandService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
        => _client.RegisterAsync(
            apiUrl: command.ApiUrl,
            apiKey: command.ApiKey,
            apiSecret: command.ApiSecret,
            memberAccount: command.PlayerCode,
            userIP: command.IpAddress,
            cancellationToken: cancellationToken);
}
