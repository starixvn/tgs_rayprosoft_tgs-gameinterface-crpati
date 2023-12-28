using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public class GameUrlGetService(IGameApiClient client) : IGetService<GameUrlQuery, GameUrlResult>
{
    public async ValueTask<GameUrlResult> GetAsync(
        GameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await client.LoginAsync(
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
