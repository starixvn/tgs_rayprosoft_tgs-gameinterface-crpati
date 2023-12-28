using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

internal class FundOutGetService(IGameApiClient client) : IGetService<FundOutQuery, FundResult>
{
    public async ValueTask<FundResult> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
    {
        var result = await client.TransferAsync(
            apiUrl: query.ApiUrl,
            apiKey: query.ApiKey,
            apiSecret: query.ApiSecret,
            memberAccount: query.PlayerCode,
            TransType.Deposit,
            amount: query.Amount,
            orderNo: query.OrderId,
            userIP: query.IpAddress,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new FundResult(
            APIOrderNo: result.APIOrderNo,
            Balance: result.Balance,
            Resp: result.Resp);
    }
}
