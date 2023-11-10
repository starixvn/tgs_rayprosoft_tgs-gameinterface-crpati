using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.CrPati.Core.Models;

namespace Tgs.GameInterface.CrPati.Core.UnitTest;

public class BetRecordByPageUnitTest
{
    [Fact]
    public async Task Unsettled()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var startDate = DateTimeOffset.Parse("2023-11-08T09:00:00+00:00");
        var endDate = DateTimeOffset.Parse("2023-11-08T10:00:00+00:00");
        var pageNumber = 1;
        var pageSize = 1000;
        var gameApiSettingsOptions = Options.Create(new GameApiSettingsOptions
        {
            Game = "CP",
            GameCode = "CPCP_Home",
            MemberPassword = "TgsPassw0rd"
        });
        var policySettingsOptions = Options.Create(new PolicySettingsOptions
        {
            BetRecordByPageRetryCount = 1,
            BetRecordByPageRetryWaitSec = 1
        });
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeCryptography = Substitute.For<IHashCryptography>();

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699434995);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetBetRecordByPageHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            startDate: Arg.Is("2023-11-08 17:00:00"),
            endDate: Arg.Is("2023-11-08 18:00:00"),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["StartDate"] = "2023-11-08 17:00:00",
            ["EndDate"] = "2023-11-08 18:00:00",
            ["PageNumber"] = pageNumber,
            ["PageSize"] = pageSize,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = true,
            ["Code"] = 0,
            ["Message"] = "执行成功",
            ["PageInfo"] = new JsonObject
            {
                ["PageNumber"] = 1,
                ["PageSize"] = 1000,
                ["TotalPageCount"] = 1,
                ["TotalRecordCount"] = 1
            },
            ["Data"] = new JsonArray {
                new JsonObject {
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
                }
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/BetRecordByPage").ToString())
            .WithJsonContent(request)
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var client = new GameApiClient(
            fakeLog,
            gameApiSettingsOptions,
            policySettingsOptions,
            fakeSystemClock,
            fakeCryptography,
            fakeHttpClient);

        var actual = await client.BetRecordByPageAsyncc(
            apiUrl: apiUrl,
            apiKey: apiKey,
            apiSecret: apiSecret,
            startDate: startDate,
            endDate: endDate,
            pageSize: pageSize,
            pageNumber: pageNumber)
            .ToArrayAsync()
            ?? Enumerable.Empty<BetRecordResult>();

        Assert.Equal("231108-31029312af", actual.First().BetID);
        Assert.Equal(0.0m, actual.First().Odds);
        Assert.Equal(0, actual.First().OddsType);
        Assert.Equal("", actual.First().GameCode);
        Assert.Equal("4ue0ujotpmuu", actual.First().MemberAccount);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T17:15:08.809+08:00"), actual.First().ModifyDate);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T17:13:01.43+08:00"), actual.First().BetDate);
        Assert.Equal(DateTimeOffset.UnixEpoch, actual.First().SettleDate);
        Assert.Equal(2.0m, actual.First().Bet);
        Assert.Equal(0.0m, actual.First().ValidBet);
        Assert.Equal(0.0m, actual.First().TotalPayout);
        Assert.Equal(0.0m, actual.First().TotalWinlose);
        Assert.Equal("Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231108045", actual.First().BetContent);
        Assert.Equal(0, actual.First().BetStatus);
        Assert.Equal(16, actual.First().GameType);
        Assert.Equal(0, actual.First().GameKind);
        Assert.Equal(response["Data"]!.AsArray()!.First()!.ToJsonString(), actual.First().AdditionalData);
    }

    [Fact]
    public async Task Settled_Lose()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var startDate = DateTimeOffset.Parse("2023-11-08T08:00:00+00:00");
        var endDate = DateTimeOffset.Parse("2023-11-08T09:00:00+00:00");
        var pageNumber = 1;
        var pageSize = 1000;
        var gameApiSettingsOptions = Options.Create(new GameApiSettingsOptions
        {
            Game = "CP",
            GameCode = "CPCP_Home",
            MemberPassword = "TgsPassw0rd"
        });
        var policySettingsOptions = Options.Create(new PolicySettingsOptions
        {
            BetRecordByPageRetryCount = 1,
            BetRecordByPageRetryWaitSec = 1
        });
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeCryptography = Substitute.For<IHashCryptography>();

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699434995);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetBetRecordByPageHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            startDate: Arg.Is("2023-11-08 16:00:00"),
            endDate: Arg.Is("2023-11-08 17:00:00"),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["StartDate"] = "2023-11-08 16:00:00",
            ["EndDate"] = "2023-11-08 17:00:00",
            ["PageNumber"] = pageNumber,
            ["PageSize"] = pageSize,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = true,
            ["Code"] = 0,
            ["Message"] = "执行成功",
            ["PageInfo"] = new JsonObject
            {
                ["PageNumber"] = 1,
                ["PageSize"] = 1000,
                ["TotalPageCount"] = 1,
                ["TotalRecordCount"] = 1
            },
            ["Data"] = new JsonArray {
                new JsonObject {
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
                }
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/BetRecordByPage").ToString())
            .WithJsonContent(request)
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var client = new GameApiClient(
            fakeLog,
            gameApiSettingsOptions,
            policySettingsOptions,
            fakeSystemClock,
            fakeCryptography,
            fakeHttpClient);

        var actual = await client.BetRecordByPageAsyncc(
            apiUrl: apiUrl,
            apiKey: apiKey,
            apiSecret: apiSecret,
            startDate: startDate,
            endDate: endDate,
            pageSize: pageSize,
            pageNumber: pageNumber)
            .ToArrayAsync()
            ?? Enumerable.Empty<BetRecordResult>();

        Assert.Equal("231108-f2df556f48", actual.First().BetID);
        Assert.Equal(0.0m, actual.First().Odds);
        Assert.Equal(0, actual.First().OddsType);
        Assert.Equal("", actual.First().GameCode);
        Assert.Equal("4ue0ujotpmuu", actual.First().MemberAccount);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T16:21:31.291+08:00"), actual.First().ModifyDate);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T15:55:33.22+08:00"), actual.First().BetDate);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T16:20:02.18+08:00"), actual.First().SettleDate);
        Assert.Equal(2.0m, actual.First().Bet);
        Assert.Equal(2.0m, actual.First().ValidBet);
        Assert.Equal(0.0m, actual.First().TotalPayout);
        Assert.Equal(-2.0m, actual.First().TotalWinlose);
        Assert.Equal("Lottery type : HighSpeedMatka20<br>Betting number : B<br>Round Code : 231108041", actual.First().BetContent);
        Assert.Equal(3, actual.First().BetStatus);
        Assert.Equal(16, actual.First().GameType);
        Assert.Equal(0, actual.First().GameKind);
        Assert.Equal(response["Data"]!.AsArray()!.First()!.ToJsonString(), actual.First().AdditionalData);
    }

    [Fact]
    public async Task Settled_Win()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var startDate = DateTimeOffset.Parse("2023-11-08T09:00:00+00:00");
        var endDate = DateTimeOffset.Parse("2023-11-08T10:00:00+00:00");
        var pageNumber = 1;
        var pageSize = 1000;
        var gameApiSettingsOptions = Options.Create(new GameApiSettingsOptions
        {
            Game = "CP",
            GameCode = "CPCP_Home",
            MemberPassword = "TgsPassw0rd"
        });
        var policySettingsOptions = Options.Create(new PolicySettingsOptions
        {
            BetRecordByPageRetryCount = 1,
            BetRecordByPageRetryWaitSec = 1
        });
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeCryptography = Substitute.For<IHashCryptography>();

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699434995);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetBetRecordByPageHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            startDate: Arg.Is("2023-11-08 17:00:00"),
            endDate: Arg.Is("2023-11-08 18:00:00"),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["StartDate"] = "2023-11-08 17:00:00",
            ["EndDate"] = "2023-11-08 18:00:00",
            ["PageNumber"] = pageNumber,
            ["PageSize"] = pageSize,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = true,
            ["Code"] = 0,
            ["Message"] = "执行成功",
            ["PageInfo"] = new JsonObject
            {
                ["PageNumber"] = 1,
                ["PageSize"] = 1000,
                ["TotalPageCount"] = 1,
                ["TotalRecordCount"] = 1
            },
            ["Data"] = new JsonArray {
                new JsonObject {
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
                }
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/BetRecordByPage").ToString())
            .WithJsonContent(request)
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var client = new GameApiClient(
            fakeLog,
            gameApiSettingsOptions,
            policySettingsOptions,
            fakeSystemClock,
            fakeCryptography,
            fakeHttpClient);

        var actual = await client.BetRecordByPageAsyncc(
            apiUrl: apiUrl,
            apiKey: apiKey,
            apiSecret: apiSecret,
            startDate: startDate,
            endDate: endDate,
            pageSize: pageSize,
            pageNumber: pageNumber)
            .ToArrayAsync()
            ?? Enumerable.Empty<BetRecordResult>();

        Assert.Equal("231108-31029312af", actual.First().BetID);
        Assert.Equal(0.0m, actual.First().Odds);
        Assert.Equal(0, actual.First().OddsType);
        Assert.Equal("", actual.First().GameCode);
        Assert.Equal("4ue0ujotpmuu", actual.First().MemberAccount);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T17:42:23.225+08:00"), actual.First().ModifyDate);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T17:13:01.43+08:00"), actual.First().BetDate);
        Assert.Equal(DateTimeOffset.Parse("2023-11-08T17:40:05.12+08:00"), actual.First().SettleDate);
        Assert.Equal(2.0m, actual.First().Bet);
        Assert.Equal(2.0m, actual.First().ValidBet);
        Assert.Equal(3.8m, actual.First().TotalPayout);
        Assert.Equal(1.8m, actual.First().TotalWinlose);
        Assert.Equal("Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231108045", actual.First().BetContent);
        Assert.Equal(3, actual.First().BetStatus);
        Assert.Equal(16, actual.First().GameType);
        Assert.Equal(0, actual.First().GameKind);
        Assert.Equal(response["Data"]!.AsArray()!.First()!.ToJsonString(), actual.First().AdditionalData);
    }

    [Fact]
    public async Task Settled_Void()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var startDate = DateTimeOffset.Parse("2023-11-09T02:00:00.000+00:00");
        var endDate = DateTimeOffset.Parse("2023-11-09T02:06:00.000+00:00");
        var pageNumber = 1;
        var pageSize = 1000;
        var gameApiSettingsOptions = Options.Create(new GameApiSettingsOptions
        {
            Game = "CP",
            GameCode = "CPCP_Home",
            MemberPassword = "TgsPassw0rd"
        });
        var policySettingsOptions = Options.Create(new PolicySettingsOptions
        {
            BetRecordByPageRetryCount = 1,
            BetRecordByPageRetryWaitSec = 1
        });
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeCryptography = Substitute.For<IHashCryptography>();

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699497670);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetBetRecordByPageHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            startDate: Arg.Is("2023-11-09 10:00:00"),
            endDate: Arg.Is("2023-11-09 10:06:00"),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["StartDate"] = "2023-11-09 10:00:00",
            ["EndDate"] = "2023-11-09 10:06:00",
            ["PageNumber"] = pageNumber,
            ["PageSize"] = pageSize,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = true,
            ["Code"] = 0,
            ["Message"] = "执行成功",
            ["PageInfo"] = new JsonObject
            {
                ["PageNumber"] = 1,
                ["PageSize"] = 1000,
                ["TotalPageCount"] = 1,
                ["TotalRecordCount"] = 1
            },
            ["Data"] = new JsonArray {
                new JsonObject {
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
                }
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/BetRecordByPage").ToString())
            .WithJsonContent(request)
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var client = new GameApiClient(
            fakeLog,
            gameApiSettingsOptions,
            policySettingsOptions,
            fakeSystemClock,
            fakeCryptography,
            fakeHttpClient);

        var actual = await client.BetRecordByPageAsyncc(
            apiUrl: apiUrl,
            apiKey: apiKey,
            apiSecret: apiSecret,
            startDate: startDate,
            endDate: endDate,
            pageSize: pageSize,
            pageNumber: pageNumber)
            .ToArrayAsync()
            ?? Enumerable.Empty<BetRecordResult>();

        Assert.Equal("231109-77055e6b9b", actual.First().BetID);
        Assert.Equal(0.0m, actual.First().Odds);
        Assert.Equal(0, actual.First().OddsType);
        Assert.Equal("", actual.First().GameCode);
        Assert.Equal("4ue0ujotpmuu", actual.First().MemberAccount);
        Assert.Equal(DateTimeOffset.Parse("2023-11-09T10:05:28.919+08:00"), actual.First().ModifyDate);
        Assert.Equal(DateTimeOffset.Parse("2023-11-09T09:46:40.633+08:00"), actual.First().BetDate);
        Assert.Equal(DateTimeOffset.UnixEpoch, actual.First().SettleDate);
        Assert.Equal(0.0m, actual.First().Bet);
        Assert.Equal(0.0m, actual.First().ValidBet);
        Assert.Equal(0.0m, actual.First().TotalPayout);
        Assert.Equal(0.0m, actual.First().TotalWinlose);
        Assert.Equal("Lottery type : HighSpeedMatka20<br>Betting number : S<br>Round Code : 231109022", actual.First().BetContent);
        Assert.Equal(2, actual.First().BetStatus);
        Assert.Equal(16, actual.First().GameType);
        Assert.Equal(0, actual.First().GameKind);
        Assert.Equal(response["Data"]!.AsArray()!.First()!.ToJsonString(), actual.First().AdditionalData);
    }

    [Fact]
    public async Task Data_is_empty()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var startDate = DateTimeOffset.Parse("2023-11-09T02:20:00+00:00");
        var endDate = DateTimeOffset.Parse("2023-11-09T02:30:00+00:00");
        var pageNumber = 1;
        var pageSize = 1000;
        var gameApiSettingsOptions = Options.Create(new GameApiSettingsOptions
        {
            Game = "CP",
            GameCode = "CPCP_Home",
            MemberPassword = "TgsPassw0rd"
        });
        var policySettingsOptions = Options.Create(new PolicySettingsOptions
        {
            BetRecordByPageRetryCount = 1,
            BetRecordByPageRetryWaitSec = 1
        });
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeCryptography = Substitute.For<IHashCryptography>();

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699498478);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetBetRecordByPageHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            startDate: Arg.Is("2023-11-09 10:20:00"),
            endDate: Arg.Is("2023-11-09 10:30:00"),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["StartDate"] = "2023-11-09 10:20:00",
            ["EndDate"] = "2023-11-09 10:30:00",
            ["PageNumber"] = pageNumber,
            ["PageSize"] = pageSize,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = true,
            ["Code"] = 0,
            ["Message"] = "执行成功",
            ["PageInfo"] = new JsonObject
            {
                ["PageNumber"] = 1,
                ["PageSize"] = 1000,
                ["TotalPageCount"] = 0,
                ["TotalRecordCount"] = 0
            },
            ["Data"] = new JsonArray { }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/BetRecordByPage").ToString())
            .WithJsonContent(request)
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var client = new GameApiClient(
            fakeLog,
            gameApiSettingsOptions,
            policySettingsOptions,
            fakeSystemClock,
            fakeCryptography,
            fakeHttpClient);

        var actual = await client.BetRecordByPageAsyncc(
            apiUrl: apiUrl,
            apiKey: apiKey,
            apiSecret: apiSecret,
            startDate: startDate,
            endDate: endDate,
            pageSize: pageSize,
            pageNumber: pageNumber)
            .ToArrayAsync()
            ?? Enumerable.Empty<BetRecordResult>();

        Assert.False(actual.Any());
    }

    [Fact]
    public async Task Retry()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var startDate = DateTimeOffset.Parse("2023-11-09T02:20:00+00:00");
        var endDate = DateTimeOffset.Parse("2023-11-09T02:30:00+00:00");
        var pageNumber = 1;
        var pageSize = 1000;
        var gameApiSettingsOptions = Options.Create(new GameApiSettingsOptions
        {
            Game = "CP",
            GameCode = "CPCP_Home",
            MemberPassword = "TgsPassw0rd"
        });
        var policySettingsOptions = Options.Create(new PolicySettingsOptions
        {
            BetRecordByPageRetryCount = 1,
            BetRecordByPageRetryWaitSec = 1
        });
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeCryptography = Substitute.For<IHashCryptography>();

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699498478);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetBetRecordByPageHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            startDate: Arg.Is("2023-11-09 10:20:00"),
            endDate: Arg.Is("2023-11-09 10:30:00"),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["StartDate"] = "2023-11-09 10:20:00",
            ["EndDate"] = "2023-11-09 10:30:00",
            ["PageNumber"] = pageNumber,
            ["PageSize"] = pageSize,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = false,
            ["Code"] = 2019,
            ["Message"] = "取得投注記錄太頻繁",
            ["PageInfo"] = new JsonObject
            {
                ["PageNumber"] = 1,
                ["PageSize"] = 1000,
                ["TotalPageCount"] = 0,
                ["TotalRecordCount"] = 0
            },
            ["Data"] = new JsonArray { }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/BetRecordByPage").ToString())
            .WithJsonContent(request)
            .Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var client = new GameApiClient(
            fakeLog,
            gameApiSettingsOptions,
            policySettingsOptions,
            fakeSystemClock,
            fakeCryptography,
            fakeHttpClient);

        _ = await Assert.ThrowsAsync<MockHttpMatchException>(
            () => client.BetRecordByPageAsyncc(
                apiUrl: apiUrl,
                apiKey: apiKey,
                apiSecret: apiSecret,
                startDate: startDate,
                endDate: endDate,
                pageSize: pageSize,
                pageNumber: pageNumber)
            .ToArrayAsync()
            .AsTask());

    }
}
