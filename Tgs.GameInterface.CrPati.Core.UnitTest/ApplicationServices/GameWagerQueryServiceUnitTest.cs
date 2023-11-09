using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CrPati.Core.ApplicationServices;
using Tgs.GameInterface.CrPati.Core.Models;

namespace Tgs.GameInterface.CrPati.Core.UnitTest.ApplicationServices;

public class GameWagerQueryServiceUnitTest
{
    [Fact]
    public async Task Unsettled()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new GameWagerQuery(
            ApiUrl: new Uri("https://apitest.fastapi2020.com/"),
            ApiKey: "testgpi123123",
            ApiSecret: "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
            StartTime: DateTimeOffset.Now.AddMinutes(-10),
            EndTime: DateTimeOffset.Now,
            PageNumber: 1,
            PageSize: 1000);

        var results = new BetRecordResult[]{
            new BetRecordResult (
                BetID : "231108-31029312af",
                Odds : 0.0m,
                OddsType : 0,
                GameCode : "",
                MemberAccount : "4ue0ujotpmuu",
                ModifyDate :DateTimeOffset.Parse( "2023-11-08T17:15:08.809+08:00"),
                BetDate : DateTimeOffset.Parse("2023-11-08T17:13:01.43+08:00"),
                SettleDate : DateTimeOffset.UnixEpoch,
                Bet : 2.0m,
                ValidBet : 0.0m,
                TotalPayout :0.0m,
                TotalWinlose : 0.0m,
                BetContent : "Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231108045",
                BetStatus : 0,
                GameType : 16,
                GameKind : 0,
                AdditionalData:new JsonObject {
                    ["BetID"] = "231108-31029312af",
                    ["Odds"] = 0.0,
                    ["OddsType"] = 0,
                    ["GameCode"] = "",
                    ["MemberAccount"] = "4ue0ujotpmuu",
                    ["ModifyDate"] = "2023-11-08T17:15:08.809+08:00",
                    ["BetDate"] = "2023-11-08T17:13:01.43+08:00",
                    ["SettleDate"] = null,
                    ["Bet"] = 2.0,
                    ["ValidBet"] = 0.0,
                    ["TotalPayout"] =0.0,
                    ["TotalWinlose"] = 0.0,
                    ["BetContent"] = "Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231108045",
                    ["BetStatus"] = 0,
                    ["GameType"] = 16,
                    ["GameKind"] = 0
                }.ToJsonString()
                ) };
        _ = fakeClient.BetRecordByPageAsyncc(
            apiUrl: Arg.Is(query.ApiUrl),
            apiKey: Arg.Is(query.ApiKey),
            apiSecret: Arg.Is(query.ApiSecret),
            startDate: Arg.Is(query.StartTime),
            endDate: Arg.Is(query.EndTime),
            pageNumber: Arg.Is(query.PageNumber),
            pageSize: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(results.ToAsyncEnumerable());

        var service = new GameWagerQueryService(
            fakeClient);

        var actual = await service.QueryAsync(
            query: query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.Equal("231108-31029312af", actual.First().ExternalWagerId);
        Assert.Equal("4ue0ujotpmuu", actual.First().PlayerCode);
        Assert.False(actual.First().IsClosed);
        Assert.False(actual.First().IsVoid);
        Assert.False(actual.First().IsCancel);
        Assert.False(actual.First().IsCashout);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T17:13:01.43+08:00"), actual.First().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.UnixEpoch, actual.First().ExternalClosedOn);
        Assert.Equal(2.0m, actual.First().Bet);
        Assert.Equal(0.0m, actual.First().ValidBet);
        Assert.Equal(0.0m, actual.First().Settle);
        Assert.Equal(0.0m, actual.First().WinLoss);
        Assert.Equal(results.First()!.AdditionalData, actual.First().AdditionalData);
    }

    [Fact]
    public async Task Settled_Lose()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new GameWagerQuery(
            ApiUrl: new Uri("https://apitest.fastapi2020.com/"),
            ApiKey: "testgpi123123",
            ApiSecret: "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
            StartTime: DateTimeOffset.Now.AddMinutes(-10),
            EndTime: DateTimeOffset.Now,
            PageNumber: 1,
            PageSize: 1000);

        var results = new BetRecordResult[]{
            new BetRecordResult (
                BetID : "231108-f2df556f48",
                Odds : 0.0m,
                OddsType : 0,
                GameCode : "",
                MemberAccount : "4ue0ujotpmuu",
                ModifyDate :DateTimeOffset.Parse( "2023-11-08T16:21:31.291+08:00"),
                BetDate : DateTimeOffset.Parse( "2023-11-08T15:55:33.22+08:00"),
                SettleDate :DateTimeOffset.Parse(  "2023-11-08T16:20:02.18+08:00"),
                Bet : 2.0m,
                ValidBet : 2.0m,
                TotalPayout : 0.0m,
                TotalWinlose : -2.0m,
                BetContent : "Lottery type : HighSpeedMatka20<br>Betting number : B<br>Round Code : 231108041",
                BetStatus : 3,
                GameType : 16,
                GameKind : 0,
                AdditionalData:new JsonObject {
                    ["BetID"] = "231108-f2df556f48",
                    ["Odds"] = 0.0,
                    ["OddsType"] = 0,
                    ["GameCode"] = "",
                    ["MemberAccount"] = "4ue0ujotpmuu",
                    ["ModifyDate"] = "2023-11-08T16:21:31.291+08:00",
                    ["BetDate"] = "2023-11-08T15:55:33.22+08:00",
                    ["SettleDate"] = "2023-11-08T16:20:02.18+08:00",
                    ["Bet"] = 2.0,
                    ["ValidBet"] = 2.0,
                    ["TotalPayout"] =0.0,
                    ["TotalWinlose"] = -2.0,
                    ["BetContent"] = "Lottery type : HighSpeedMatka20<br>Betting number : B<br>Round Code : 231108041",
                    ["BetStatus"] = 3,
                    ["GameType"] = 16,
                    ["GameKind"] = 0
                }.ToJsonString()
                ) };
        _ = fakeClient.BetRecordByPageAsyncc(
            apiUrl: Arg.Is(query.ApiUrl),
            apiKey: Arg.Is(query.ApiKey),
            apiSecret: Arg.Is(query.ApiSecret),
            startDate: Arg.Is(query.StartTime),
            endDate: Arg.Is(query.EndTime),
            pageNumber: Arg.Is(query.PageNumber),
            pageSize: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(results.ToAsyncEnumerable());

        var service = new GameWagerQueryService(
            fakeClient);

        var actual = await service.QueryAsync(
            query: query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.Equal("231108-f2df556f48", actual.First().ExternalWagerId);
        Assert.Equal("4ue0ujotpmuu", actual.First().PlayerCode);
        Assert.True(actual.First().IsClosed);
        Assert.False(actual.First().IsVoid);
        Assert.False(actual.First().IsCancel);
        Assert.False(actual.First().IsCashout);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T15:55:33.22+08:00"), actual.First().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T16:20:02.18+08:00"), actual.First().ExternalClosedOn);
        Assert.Equal(2.0m, actual.First().Bet);
        Assert.Equal(2.0m, actual.First().ValidBet);
        Assert.Equal(0.0m, actual.First().Settle);
        Assert.Equal(-2.0m, actual.First().WinLoss);
        Assert.Equal(results.First()!.AdditionalData, actual.First().AdditionalData);
    }

    [Fact]
    public async Task Settled_Win()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new GameWagerQuery(
            ApiUrl: new Uri("https://apitest.fastapi2020.com/"),
            ApiKey: "testgpi123123",
            ApiSecret: "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
            StartTime: DateTimeOffset.Now.AddMinutes(-10),
            EndTime: DateTimeOffset.Now,
            PageNumber: 1,
            PageSize: 1000);

        var results = new BetRecordResult[]{
            new BetRecordResult (
                BetID : "231108-31029312af",
                Odds : 0.0m,
                OddsType : 0,
                GameCode : "",
                MemberAccount : "4ue0ujotpmuu",
                ModifyDate : DateTimeOffset.Parse("2023-11-08T17:42:23.225+08:00"),
                BetDate :  DateTimeOffset.Parse("2023-11-08T17:13:01.43+08:00"),
                SettleDate : DateTimeOffset.Parse( "2023-11-08T17:40:05.12+08:00"),
                Bet : 2.0m,
                ValidBet : 2.0m,
                TotalPayout : 3.8m,
                TotalWinlose : 1.8m,
                BetContent : "Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231108045",
                BetStatus : 3,
                GameType : 16,
                GameKind : 0,
                AdditionalData:new JsonObject {
                    ["BetID"] = "231108-31029312af",
                    ["Odds"] = 0.0,
                    ["OddsType"] = 0,
                    ["GameCode"] = "",
                    ["MemberAccount"] = "4ue0ujotpmuu",
                    ["ModifyDate"] = "2023-11-08T17:42:23.225+08:00",
                    ["BetDate"] = "2023-11-08T17:13:01.43+08:00",
                    ["SettleDate"] = "2023-11-08T17:40:05.12+08:00",
                    ["Bet"] = 2.0,
                    ["ValidBet"] = 2.0,
                    ["TotalPayout"] =3.8,
                    ["TotalWinlose"] = 1.8,
                    ["BetContent"] = "Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231108045",
                    ["BetStatus"] = 3,
                    ["GameType"] = 16,
                    ["GameKind"] = 0
                }.ToJsonString()
                ) };
        _ = fakeClient.BetRecordByPageAsyncc(
            apiUrl: Arg.Is(query.ApiUrl),
            apiKey: Arg.Is(query.ApiKey),
            apiSecret: Arg.Is(query.ApiSecret),
            startDate: Arg.Is(query.StartTime),
            endDate: Arg.Is(query.EndTime),
            pageNumber: Arg.Is(query.PageNumber),
            pageSize: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(results.ToAsyncEnumerable());

        var service = new GameWagerQueryService(
            fakeClient);

        var actual = await service.QueryAsync(
            query: query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.Equal("231108-31029312af", actual.First().ExternalWagerId);
        Assert.Equal("4ue0ujotpmuu", actual.First().PlayerCode);
        Assert.True(actual.First().IsClosed);
        Assert.False(actual.First().IsVoid);
        Assert.False(actual.First().IsCancel);
        Assert.False(actual.First().IsCashout);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T17:13:01.43+08:00"), actual.First().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T17:40:05.12+08:00"), actual.First().ExternalClosedOn);
        Assert.Equal(2.0m, actual.First().Bet);
        Assert.Equal(2.0m, actual.First().ValidBet);
        Assert.Equal(3.8m, actual.First().Settle);
        Assert.Equal(1.8m, actual.First().WinLoss);
        Assert.Equal(results.First()!.AdditionalData, actual.First().AdditionalData);
    }

    [Fact]
    public async Task Settled_Void()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new GameWagerQuery(
            ApiUrl: new Uri("https://apitest.fastapi2020.com/"),
            ApiKey: "testgpi123123",
            ApiSecret: "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
            StartTime: DateTimeOffset.Now.AddMinutes(-10),
            EndTime: DateTimeOffset.Now,
            PageNumber: 1,
            PageSize: 1000);

        var results = new BetRecordResult[]{
            new BetRecordResult (
                BetID : "231109-77055e6b9b",
                Odds : 0.0m,
                OddsType : 0,
                GameCode : "",
                MemberAccount : "4ue0ujotpmuu",
                ModifyDate :DateTimeOffset.Parse( "2023-11-09T10:05:28.919+08:00"),
                BetDate :DateTimeOffset.Parse( "2023-11-09T09:46:40.633+08:00"),
                SettleDate : DateTimeOffset.UnixEpoch,
                Bet : 0.0m,
                ValidBet : 0.0m,
                TotalPayout: 0.0m,
                TotalWinlose : 0.0m,
                BetContent : "Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231109022",
                BetStatus : 2,
                GameType : 16,
                GameKind : 0,
                AdditionalData:new JsonObject {
                    ["BetID"] = "231109-77055e6b9b",
                    ["Odds"] = 0.0,
                    ["OddsType"] = 0,
                    ["GameCode"] = "",
                    ["MemberAccount"] = "4ue0ujotpmuu",
                    ["ModifyDate"] = "2023-11-09T10:05:28.919+08:00",
                    ["BetDate"] = "2023-11-09T09:46:40.633+08:00",
                    ["SettleDate"] = null,
                    ["Bet"] = 0.0,
                    ["ValidBet"] = 0.0,
                    ["TotalPayout"] =0.0,
                    ["TotalWinlose"] = 0.0,
                    ["BetContent"] = "Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231109022",
                    ["BetStatus"] = 2,
                    ["GameType"] = 16,
                    ["GameKind"] = 0
                }.ToJsonString()
                ) };
        _ = fakeClient.BetRecordByPageAsyncc(
            apiUrl: Arg.Is(query.ApiUrl),
            apiKey: Arg.Is(query.ApiKey),
            apiSecret: Arg.Is(query.ApiSecret),
            startDate: Arg.Is(query.StartTime),
            endDate: Arg.Is(query.EndTime),
            pageNumber: Arg.Is(query.PageNumber),
            pageSize: Arg.Is(query.PageSize),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(results.ToAsyncEnumerable());

        var service = new GameWagerQueryService(
            fakeClient);

        var actual = await service.QueryAsync(
            query: query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.Equal("231109-77055e6b9b", actual.First().ExternalWagerId);
        Assert.Equal("4ue0ujotpmuu", actual.First().PlayerCode);
        Assert.True(actual.First().IsClosed);
        Assert.True(actual.First().IsVoid);
        Assert.False(actual.First().IsCancel);
        Assert.False(actual.First().IsCashout);
        Assert.Equal(DateTimeOffset.Parse("2023-11-09T09:46:40.633+08:00"), actual.First().ExternalCreatedOn);
        Assert.Equal(DateTimeOffset.Parse("2023-11-09T10:05:28.919+08:00"), actual.First().ExternalClosedOn);
        Assert.Equal(0.0m, actual.First().Bet);
        Assert.Equal(0.0m, actual.First().ValidBet);
        Assert.Equal(0.0m, actual.First().Settle);
        Assert.Equal(0.0m, actual.First().WinLoss);
        Assert.Equal(results.First()!.AdditionalData, actual.First().AdditionalData);
    }
}
