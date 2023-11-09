using Tgs.GameInterface.CrPati.Protos;

namespace Tgs.GameInterface.CrPati.Client;

public interface IGameInterfaceCrPatiClient
{
    ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default);

    ValueTask<FundReply> FundInAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, string orderId, decimal amount, CancellationToken cancellationToken = default);

    ValueTask<FundReply> FundOutAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, string orderId, decimal amount, CancellationToken cancellationToken = default);

    ValueTask<bool> FundStatusAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, string orderId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<GameWagerInfo> GameWagerAsync(Uri apiUrl, string apiKey, string apiSecret, DateTimeOffset startTime, DateTimeOffset endTime, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, int lang, bool isMobile, CancellationToken cancellationToken = default);

    ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, CancellationToken cancellationToken = default);

    ValueTask PlayerCreateAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, CancellationToken cancellationToken = default);
}
