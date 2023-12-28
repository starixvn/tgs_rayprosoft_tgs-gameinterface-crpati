using System.Runtime.CompilerServices;
using Tgs.GameInterface.CrPati.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

internal class GameWagerQueryService(
    IGameApiClient client) : IQueryService<GameWagerQuery, GameWagerResult>
{
    public async IAsyncEnumerable<GameWagerResult> QueryAsync(
        GameWagerQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var betRecords = await client.BetRecordByPageAsyncc(
            apiUrl: query.ApiUrl,
            apiKey: query.ApiKey,
            apiSecret: query.ApiSecret,
            startDate: query.StartTime,
            endDate: query.EndTime,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize,
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<BetRecordResult>();

        if (!betRecords.Any())
        {
            yield break;
        }

        foreach (var betRecord in betRecords)
        {
            var bet = betRecord.Bet;
            var validBet = betRecord.ValidBet;
            var settle = 0.0m;
            var winLoss = 0.0m;
            var isClosed = betRecord.BetStatus != 0;
            var isVoid = false;
            var isCancel = false;
            var isCashout = false;
            var externalCreatedOn = betRecord.BetDate;
            var externalClosedOn = DateTimeOffset.UnixEpoch;
            if (isClosed)
            {
                settle = betRecord.TotalPayout;
                winLoss = betRecord.TotalWinlose;
                externalClosedOn = betRecord.SettleDate;
                isVoid = betRecord.BetStatus == 2;
                isCancel = betRecord.BetStatus == 1;
                isCashout = betRecord.BetStatus == 4;
                if (isVoid || isCancel)
                {
                    externalClosedOn = betRecord.ModifyDate;
                }
            }
            yield return new GameWagerResult(
                ExternalWagerId: betRecord.BetID,
                PlayerCode: betRecord.MemberAccount,
                IsClosed: isClosed,
                IsVoid: isVoid,
                IsCancel: isCancel,
                IsCashout: isCashout,
                ExternalCreatedOn: externalCreatedOn,
                ExternalClosedOn: externalClosedOn,
                Bet: bet,
                ValidBet: validBet,
                Settle: settle,
                WinLoss: winLoss,
                AdditionalData: betRecord.AdditionalData);
        }
    }
}
