using Tgs.GameInterface.CrPati.Core.Models;

namespace Tgs.GameInterface.CrPati.Core;

public interface IGameApiClient
{
    ValueTask<LoginResult> LoginAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, string userIP, int languageCode, bool isMobile, bool isTrial, CancellationToken cancellationToken = default);

    ValueTask RegisterAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, string userIP, CancellationToken cancellationToken = default);

    ValueTask<BalanceResult> BalanceAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, string userIP, CancellationToken cancellationToken = default);

    ValueTask<TransferResult> TransferAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, TransType transType, decimal amount, string orderNo, string userIP, CancellationToken cancellationToken = default);

    ValueTask<bool> CheckTransferAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, string orderNo, string userIP, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BetRecordResult> BetRecordByPageAsyncc(Uri apiUrl, string apiKey, string apiSecret, DateTimeOffset startDate, DateTimeOffset endDate, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default);
}
