using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.CrPati.Protos;

namespace Tgs.GameInterface.CrPati.Client;

internal class GameInterfaceCrPatiClient : IGameInterfaceCrPatiClient
{
    private readonly GameInterfaceCrPati.GameInterfaceCrPatiClient _client;

    public GameInterfaceCrPatiClient(GameInterfaceCrPati.GameInterfaceCrPatiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default)
    {
        var request = new CheckingServiceRequest
        {
            MachineName = machineName,
        };

        var reply = await _client
            .CheckingServiceAsync(
                request: request,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return reply.MachineName;
    }

    public async ValueTask<FundReply> FundInAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var request = new FundRequest
        {
            ApiUrl = apiUrl.ToString(),
            ApiKey = apiKey,
            ApiSecret = apiSecret,
            PlayerCode = playerCode,
            IpAddress = ipAddress,
            OrderId = orderId,
            Amount = amount
        };

        return await _client.FundInAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<FundReply> FundOutAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var request = new FundRequest
        {
            ApiUrl = apiUrl.ToString(),
            ApiKey = apiKey,
            ApiSecret = apiSecret,
            PlayerCode = playerCode,
            IpAddress = ipAddress,
            OrderId = orderId,
            Amount = amount
        };

        return await _client.FundOutAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<bool> FundStatusAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, string orderId, CancellationToken cancellationToken = default)
    {
        var request = new FundStatusRequest
        {
            ApiUrl = apiUrl.ToString(),
            ApiKey = apiKey,
            ApiSecret = apiSecret,
            PlayerCode = playerCode,
            IpAddress = ipAddress,
            OrderId = orderId
        };

        var reply = await _client.FundStatusAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return reply.State;
    }

    public async ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, int lang, bool isMobile, CancellationToken cancellationToken = default)
    {
        var request = new GameUrlRequest
        {
            ApiUrl = apiUrl.ToString(),
            ApiKey = apiKey,
            ApiSecret = apiSecret,
            PlayerCode = playerCode,
            IpAddress = ipAddress,
            Lang = lang,
            IsMobile = isMobile,
        };

        return await _client.GameUrlAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async IAsyncEnumerable<GameWagerInfo> GameWagerAsync(Uri apiUrl, string apiKey, string apiSecret, DateTimeOffset startTime, DateTimeOffset endTime, int pageNumber, int pageSize, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new GameWagerRequest
        {
            ApiUrl = apiUrl.ToString(),
            ApiKey = apiKey,
            ApiSecret = apiSecret,
            StartTime = startTime.ToTimestamp(),
            EndTime = endTime.ToTimestamp(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var reply = await _client.GameWagerAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var infos = reply.Items ?? Enumerable.Empty<GameWagerInfo>();

        if (!infos.Any())
        {
            yield break;
        }

        foreach (var info in infos)
        {
            yield return info;
        }
    }

    public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, CancellationToken cancellationToken = default)
    {
        var request = new PlayerBalanceRequest
        {
            ApiUrl = apiUrl.ToString(),
            ApiKey = apiKey,
            ApiSecret = apiSecret,
            PlayerCode = playerCode,
            IpAddress = ipAddress
        };

        return await _client.PlayerBalanceAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask PlayerCreateAsync(Uri apiUrl, string apiKey, string apiSecret, string playerCode, string ipAddress, CancellationToken cancellationToken = default)
    {
        var request = new PlayerCreateRequest
        {
            ApiUrl = apiUrl.ToString(),
            ApiKey = apiKey,
            ApiSecret = apiSecret,
            PlayerCode = playerCode,
            IpAddress = ipAddress
        };

        _ = await _client.PlayerCreateAsync(
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
