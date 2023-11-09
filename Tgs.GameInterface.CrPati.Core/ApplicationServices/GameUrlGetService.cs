using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public class GameUrlGetService : IGetService<GameUrlQuery, GameUrlResult>
{
    private readonly IGameApiClient _client;

    public GameUrlGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        GameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _client.LoginAsync(
            apiUrl: query.ApiUrl,
            apiKey: query.ApiKey,
            apiSecret: query.ApiSecret,
            memberAccount: query.PlayerCode,
            userIP: query.IpAddress,
            languageCode: query.Lang,
            isMobile: query.IsMobile,
            isTrial: false,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new GameUrlResult(
            IsPlayerExist: result.IsPlayerExist,
            GameUrl: result.GameUrl);
    }
}
