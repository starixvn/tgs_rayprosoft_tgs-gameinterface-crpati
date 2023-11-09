using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.CrPati.Core.ApplicationServices;
using Tgs.GameInterface.CrPati.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.GrpcServices;

public class GameInterfaceCrPatiGrpcService : GameInterfaceCrPati.GameInterfaceCrPatiBase
{
    private readonly IGetService<FundInQuery, FundResult> _fundInService;
    private readonly IGetService<FundOutQuery, FundResult> _fundOutService;
    private readonly IGetService<FundStatusQuery, bool> _fundStatusService;
    private readonly IQueryService<GameWagerQuery, GameWagerResult> _gameWagerService;
    private readonly IGetService<GameUrlQuery, GameUrlResult> _gameUrlService;
    private readonly ILogger<GameInterfaceCrPatiGrpcService> _logger;
    private readonly IGetService<PlayerBalanceQuery, PlayerBalanceResult> _playerBalanceService;
    private readonly ICommandService<PlayerCreateCommand> _playerCreateService;

    public GameInterfaceCrPatiGrpcService(
        ILogger<GameInterfaceCrPatiGrpcService> logger,
        IGetService<FundInQuery, FundResult> fundInService,
        IGetService<FundOutQuery, FundResult> fundOutService,
        IGetService<FundStatusQuery, bool> fundStatusService,
        IQueryService<GameWagerQuery, GameWagerResult> gameWagerService,
        IGetService<GameUrlQuery, GameUrlResult> gameUrlService,
        IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
        ICommandService<PlayerCreateCommand> playerCreateService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fundInService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        _fundOutService = fundOutService ?? throw new ArgumentNullException(nameof(fundOutService));
        _fundStatusService = fundStatusService ?? throw new ArgumentNullException(nameof(fundStatusService));
        _gameWagerService = gameWagerService ?? throw new ArgumentNullException(nameof(gameWagerService));
        _gameUrlService = gameUrlService ?? throw new ArgumentNullException(nameof(gameUrlService));
        _playerBalanceService = playerBalanceService ?? throw new ArgumentNullException(nameof(playerBalanceService));
        _playerCreateService = playerCreateService ?? throw new ArgumentNullException(nameof(playerCreateService));
    }

    public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
    {
        _logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

        return Task.FromResult(new CheckingServiceReply
        {
            MachineName = Environment.MachineName
        });
    }

    public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
    {
        var query = new FundInQuery(
            ApiUrl: new(request.ApiUrl),
            ApiKey: request.ApiKey,
            ApiSecret: request.ApiSecret,
            PlayerCode: request.PlayerCode,
            IpAddress: request.IpAddress,
            OrderId: request.OrderId,
            Amount: request.Amount);

        var result = await _fundInService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply
        {
            ApiOrderNo = result.APIOrderNo,
            Balance = result.Balance,
            Resp = result.Resp,
        };
    }

    public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
    {
        var query = new FundOutQuery(
            ApiUrl: new(request.ApiUrl),
            ApiKey: request.ApiKey,
            ApiSecret: request.ApiSecret,
            PlayerCode: request.PlayerCode,
            IpAddress: request.IpAddress,
            OrderId: request.OrderId,
            Amount: request.Amount);

        var result = await _fundOutService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply
        {
            ApiOrderNo = result.APIOrderNo,
            Balance = result.Balance,
            Resp = result.Resp,
        };
    }

    public override async Task<BaseReply> FundStatus(FundStatusRequest request, ServerCallContext context)
    {
        var query = new FundStatusQuery(
            ApiUrl: new(request.ApiUrl),
            ApiKey: request.ApiKey,
            ApiSecret: request.ApiSecret,
            PlayerCode: request.PlayerCode,
            IpAddress: request.IpAddress,
            OrderId: request.OrderId);

        var result = await _fundStatusService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            State = result
        };
    }

    public override async Task<GameUrlReply> GameUrl(GameUrlRequest request, ServerCallContext context)
    {
        var query = new GameUrlQuery(
            ApiUrl: new(request.ApiUrl),
            ApiKey: request.ApiKey,
            ApiSecret: request.ApiSecret,
            PlayerCode: request.PlayerCode,
            IpAddress: request.IpAddress,
            Lang: request.Lang,
            IsMobile: request.IsMobile);

        var result = await _gameUrlService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new GameUrlReply
        {
            IsPlayerExist = result.IsPlayerExist,
            GameUrl = result.GameUrl
        };
    }

    public override async Task<GameWagerReply> GameWager(GameWagerRequest request, ServerCallContext context)
    {
        var query = new GameWagerQuery(
            ApiUrl: new(request.ApiUrl),
            ApiKey: request.ApiKey,
            ApiSecret: request.ApiSecret,
            StartTime: request.StartTime.ToDateTimeOffset(),
            EndTime: request.EndTime.ToDateTimeOffset(),
            PageNumber: request.PageNumber,
            PageSize: request.PageSize);

        var results = await _gameWagerService.QueryAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ToArrayAsync(
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        var reply = new GameWagerReply();

        if (results.Any())
        {
            foreach (var result in results)
            {
                reply.Items.Add(new GameWagerInfo
                {
                    ExternalWagerId = result.ExternalWagerId,
                    PlayerCode = result.PlayerCode,
                    IsClosed = result.IsClosed,
                    IsVoid = result.IsVoid,
                    IsCancel = result.IsCancel,
                    IsCashout = result.IsCashout,
                    ExternalCreatedOn = result.ExternalCreatedOn.ToTimestamp(),
                    ExternalClosedOn = result.ExternalClosedOn.ToTimestamp(),
                    Bet = result.Bet,
                    ValidBet = result.ValidBet,
                    Settle = result.Settle,
                    WinLoss = result.WinLoss,
                    AdditionalData = result.AdditionalData
                });
            }
        }

        return reply;
    }

    public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
    {
        var query = new PlayerBalanceQuery(
            ApiUrl: new(request.ApiUrl),
            ApiKey: request.ApiKey,
            ApiSecret: request.ApiSecret,
            PlayerCode: request.PlayerCode,
            IpAddress: request.IpAddress);

        var result = await _playerBalanceService.GetAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new PlayerBalanceReply
        {
            IsPlayerExist = result.IsPlayerExist,
            Balance = result.Balance
        };
    }

    public override async Task<BaseReply> PlayerCreate(PlayerCreateRequest request, ServerCallContext context)
    {
        var command = new PlayerCreateCommand(
            ApiUrl: new(request.ApiUrl),
            ApiKey: request.ApiKey,
            ApiSecret: request.ApiSecret,
            PlayerCode: request.PlayerCode,
            IpAddress: request.IpAddress);

        await _playerCreateService.ExecuteAsync(
            command: command,
            cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            State = true
        };
    }
}
