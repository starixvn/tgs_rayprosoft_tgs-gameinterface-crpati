using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public class FundStatusGetService : IGetService<FundStatusQuery, bool>
{
    private readonly IGameApiClient _client;

    public FundStatusGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<bool> GetAsync(
        FundStatusQuery query,
        CancellationToken cancellationToken = default)
        => _client.CheckTransferAsync(
            apiUrl: query.ApiUrl,
            apiKey: query.ApiKey,
            apiSecret: query.ApiSecret,
            memberAccount: query.PlayerCode,
            orderNo: query.OrderId,
            userIP: query.IpAddress,
            cancellationToken: cancellationToken);
}
