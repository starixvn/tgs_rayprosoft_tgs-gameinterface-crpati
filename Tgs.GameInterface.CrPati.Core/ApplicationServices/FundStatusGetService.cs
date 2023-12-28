using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public class FundStatusGetService(IGameApiClient client) : IGetService<FundStatusQuery, bool>
{
    public ValueTask<bool> GetAsync(
        FundStatusQuery query,
        CancellationToken cancellationToken = default)
        => client.CheckTransferAsync(
            apiUrl: query.ApiUrl,
            apiKey: query.ApiKey,
            apiSecret: query.ApiSecret,
            memberAccount: query.PlayerCode,
            orderNo: query.OrderId,
            userIP: query.IpAddress,
            cancellationToken: cancellationToken);
}
