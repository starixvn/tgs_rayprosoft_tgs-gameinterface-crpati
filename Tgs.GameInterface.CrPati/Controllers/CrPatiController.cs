using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.CrPati.Core.ApplicationServices;
using Tgs.GameInterface.CrPati.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.Controllers;

[Route("[controller]")]
[ApiController]
public class CrPatiController : ControllerBase
{
    /// <summary>
    /// Funds the in asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://apitest.fastapi2020.com/",
    ///             "apiKey": "testgpi123123",
    ///             "apiSecret": "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
    ///             "playerCode":"4ue0ujotpmuu",
    ///             "ipAddress": "127.0.0.1",
    ///             "orderId": "bb1b2e47fb3a4fab97ec00deb731f9fa",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("fund/in", Name = "FundIn")]
    public async ValueTask<FundResultViewModel> FundInAsync(
        [FromServices] IGetService<FundInQuery, FundResult> service,
        FundViewModel source)
    {
        var query = new FundInQuery(
            ApiUrl: source.ApiUrl,
            ApiKey: source.ApiKey,
            ApiSecret: source.ApiSecret,
            PlayerCode: source.PlayerCode,
            IpAddress: source.IpAddress,
            OrderId: source.OrderId,
            Amount: source.Amount);

        var result = await service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new FundResultViewModel(result.Resp);
    }

    /// <summary>
    /// Funds the out asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://apitest.fastapi2020.com/",
    ///             "apiKey": "testgpi123123",
    ///             "apiSecret": "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
    ///             "playerCode":"4ue0ujotpmuu",
    ///             "ipAddress": "127.0.0.1",
    ///             "orderId": "a07fb0047df74ced9ba07c93ba152458",
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("fund/out", Name = "FundOut")]
    public async ValueTask<FundResultViewModel> FundOutAsync(
        [FromServices] IGetService<FundOutQuery, FundResult> service,
        FundViewModel source)
    {
        var query = new FundOutQuery(
            ApiUrl: source.ApiUrl,
            ApiKey: source.ApiKey,
            ApiSecret: source.ApiSecret,
            PlayerCode: source.PlayerCode,
            IpAddress: source.IpAddress,
            OrderId: source.OrderId,
            Amount: source.Amount);

        var result = await service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new FundResultViewModel(result.Resp);
    }

    /// <summary>
    /// Funds the status asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://apitest.fastapi2020.com/",
    ///             "apiKey": "testgpi123123",
    ///             "apiSecret": "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
    ///             "playerCode":"4ue0ujotpmuu",
    ///             "ipAddress": "127.0.0.1",
    ///             "orderId": "a07fb0047df74ced9ba07c93ba152458"
    ///         }
    /// </remarks>
    [HttpPost("fund/Status", Name = "FundStatus")]
    public ValueTask<bool> FundStatusAsync(
        [FromServices] IGetService<FundStatusQuery, bool> service,
        FundStatusViewModel source)
    {
        var query = new FundStatusQuery(
            ApiUrl: source.ApiUrl,
            ApiKey: source.ApiKey,
            ApiSecret: source.ApiSecret,
            PlayerCode: source.PlayerCode,
            IpAddress: source.IpAddress,
            OrderId: source.OrderId);

        return service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted);
    }

    /// <summary>
    /// Games the wager asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://apitest.fastapi2020.com/",
    ///             "apiKey": "testgpi123123",
    ///             "apiSecret": "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
    ///             "startTime": "2023-11-08T13:00:00+08:00",
    ///             "endTime": "2023-11-08T14:00:00+08:00",
    ///             "pageNumber": 1,
    ///             "pageSize": 1000
    ///
    ///         }
    /// </remarks>
    [HttpPost("game/wager")]
    public async IAsyncEnumerable<GameWagerResultViewModel> GameWagerAsync(
        [FromServices] IQueryService<GameWagerQuery, GameWagerResult> service,
        GameWagerViewModel source)
    {
        var query = new GameWagerQuery(
            ApiUrl: source.ApiUrl,
            ApiKey: source.ApiKey,
            ApiSecret: source.ApiSecret,
            StartTime: source.StartTime,
            EndTime: source.EndTime,
            PageNumber: source.PageNumber,
            PageSize: source.PageSize);

        var results = await service
            .QueryAsync(
                query: query,
                cancellationToken: HttpContext.RequestAborted)
            .ToArrayAsync(
                cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        if (!results.Any())
        {
            yield break;
        }

        foreach (var result in results)
        {
            yield return new GameWagerResultViewModel(
                result.ExternalWagerId,
                result.PlayerCode,
                result.IsClosed,
                result.IsVoid,
                result.IsCancel,
                result.IsCashout,
                result.ExternalCreatedOn,
                result.ExternalClosedOn,
                result.Bet,
                result.ValidBet,
                result.Settle,
                result.WinLoss,
                result.AdditionalData);
        }
    }

    /// <summary>
    /// Games the URL asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://apitest.fastapi2020.com/",
    ///             "apiKey": "testgpi123123",
    ///             "apiSecret": "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
    ///             "playerCode":"4ue0ujotpmuu",
    ///             "ipAddress": "127.0.0.1",
    ///             "lang": 0,
    ///             "isMobile": false
    ///         }
    /// </remarks>
    [HttpPost("gameurl")]
    public async ValueTask<GameUrlResultViewModel> GameUrlAsync(
        [FromServices] IGetService<GameUrlQuery, GameUrlResult> service,
        GameUrlViewModel source)
    {
        var query = new GameUrlQuery(
            ApiUrl: source.ApiUrl,
            ApiKey: source.ApiKey,
            ApiSecret: source.ApiSecret,
            PlayerCode: source.PlayerCode,
            IpAddress: source.IpAddress,
            Lang: source.Lang,
            IsMobile: source.IsMobile);

        var result = await service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new GameUrlResultViewModel(
            result.IsPlayerExist,
            result.GameUrl);
    }

    /// <summary>
    /// Players the balance asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://apitest.fastapi2020.com/",
    ///             "apiKey": "testgpi123123",
    ///             "apiSecret": "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
    ///             "playerCode":"4ue0ujotpmuu",
    ///             "ipAddress": "127.0.0.1"
    ///         }
    /// </remarks>
    [HttpPost("player/balance", Name = "PlayerBalance")]
    public async ValueTask<PlayerBalanceResultViewModel> PlayerBalanceAsync(
        [FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
        PlayerBalanceViewModel source)
    {
        var query = new PlayerBalanceQuery(
            ApiUrl: source.ApiUrl,
            ApiKey: source.ApiKey,
            ApiSecret: source.ApiSecret,
            PlayerCode: source.PlayerCode,
            IpAddress: source.IpAddress);

        var result = await service.GetAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false);

        return new PlayerBalanceResultViewModel(
            IsPlayerExist: result.IsPlayerExist,
            Balance: result.Balance);
    }

    /// <summary>
    /// Players the create asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="source">The source.</param>
    /// <returns></returns>
    /// <remarks>
    /// <b>Example</b><br />
    ///
    ///     Post
    ///
    ///         {
    ///             "apiUrl": "https://apitest.fastapi2020.com/",
    ///             "apiKey": "testgpi123123",
    ///             "apiSecret": "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
    ///             "playerCode":"4ue0ujotpmuu",
    ///             "ipAddress": "127.0.0.1"
    ///         }
    /// </remarks>
    [HttpPost("player/create", Name = "PlayerCreate")]
    public ValueTask PlayerCreateAsync(
        [FromServices] ICommandService<PlayerCreateCommand> service,
        PlayerCreateViewModel source)
    {
        var command = new PlayerCreateCommand(
            ApiUrl: source.ApiUrl,
            ApiKey: source.ApiKey,
            ApiSecret: source.ApiSecret,
            PlayerCode: source.PlayerCode,
            IpAddress: source.IpAddress);

        return service.ExecuteAsync(
                command: command,
                cancellationToken: HttpContext.RequestAborted);
    }
}
