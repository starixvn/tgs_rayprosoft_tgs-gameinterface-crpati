using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.CrPati.Core.Exceptions;

namespace Tgs.GameInterface.CrPati.Core.UnitTest;

public class TransferUnitTest
{
    [Fact]
    public async Task Player_does_not_exist()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var memberAccount = "4ue0ujotpmuu";
        var userIP = "127.0.0.1";
        var transType = TransType.Deposit;
        var amount = 1.0m;
        var orderNo = "bb1b2e47fb3a4fab97ec00deb731f9fa";

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

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699426193);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetTransferHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            memberAccount: Arg.Is(memberAccount),
            memberPassword: Arg.Is(gameApiSettingsOptions.Value.MemberPassword),
            orderNo: Arg.Is(orderNo),
            transType: Arg.Is(transType.ToString()),
            amount: Arg.Is(amount),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["MemberAccount"] = memberAccount,
            ["MemberPassword"] = gameApiSettingsOptions.Value.MemberPassword,
            ["TransType"] = transType.ToString(),
            ["Amount"] = amount,
            ["OrderNo"] = orderNo,
            ["UserIP"] = userIP,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = false,
            ["Code"] = 2008,
            ["Message"] = "会员不存在",
            ["Data"] = null
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/Transfer").ToString())
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

        _ = await Assert.ThrowsAsync<ReturnCodeException>(
            () => client.TransferAsync(
              apiUrl: apiUrl,
              apiKey: apiKey,
              apiSecret: apiSecret,
              memberAccount: memberAccount,
              transType: transType,
              amount: amount,
              orderNo: orderNo,
              userIP: userIP)
            .AsTask());
    }

    [Fact]
    public async Task Normal_Process_Deposit()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var memberAccount = "4ue0ujotpmuu";
        var userIP = "127.0.0.1";
        var transType = TransType.Deposit;
        var amount = 1000.0m;
        var orderNo = "a07fb0047df74ced9ba07c93ba152458";

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

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699429332);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetTransferHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            memberAccount: Arg.Is(memberAccount),
            memberPassword: Arg.Is(gameApiSettingsOptions.Value.MemberPassword),
            orderNo: Arg.Is(orderNo),
            transType: Arg.Is(transType.ToString()),
            amount: Arg.Is(amount),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["MemberAccount"] = memberAccount,
            ["MemberPassword"] = gameApiSettingsOptions.Value.MemberPassword,
            ["TransType"] = transType.ToString(),
            ["Amount"] = amount,
            ["OrderNo"] = orderNo,
            ["UserIP"] = userIP,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = true,
            ["Code"] = 0,
            ["Message"] = "执行成功",
            ["Data"] = new JsonObject
            {
                ["APIOrderNo"] = "2311081542127340855",
                ["Balance"] = 1000.0
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/Transfer").ToString())
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

        var actual = await client.TransferAsync(
            apiUrl: apiUrl,
            apiKey: apiKey,
            apiSecret: apiSecret,
            memberAccount: memberAccount,
            transType: transType,
            amount: amount,
            orderNo: orderNo,
            userIP: userIP);

        Assert.Equal("2311081542127340855", actual.APIOrderNo);
        Assert.Equal(1000.0m, actual.Balance);
        Assert.Equal(response.ToJsonString(), actual.Resp);
    }

    [Fact]
    public async Task Normal_Process_Withdraw()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var memberAccount = "4ue0ujotpmuu";
        var userIP = "127.0.0.1";
        var transType = TransType.Withdraw;
        var amount = 1.0m;
        var orderNo = "bb1b2e47fb3a4fab97ec00deb731f9fa";

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

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699429332);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetTransferHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            memberAccount: Arg.Is(memberAccount),
            memberPassword: Arg.Is(gameApiSettingsOptions.Value.MemberPassword),
            orderNo: Arg.Is(orderNo),
            transType: Arg.Is(transType.ToString()),
            amount: Arg.Is(amount),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["MemberAccount"] = memberAccount,
            ["MemberPassword"] = gameApiSettingsOptions.Value.MemberPassword,
            ["TransType"] = transType.ToString(),
            ["Amount"] = amount,
            ["OrderNo"] = orderNo,
            ["UserIP"] = userIP,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = true,
            ["Code"] = 0,
            ["Message"] = "执行成功",
            ["Data"] = new JsonObject
            {
                ["APIOrderNo"] = "2311081546291049092",
                ["Balance"] = 999.0
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/Transfer").ToString())
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

        var actual = await client.TransferAsync(
            apiUrl: apiUrl,
            apiKey: apiKey,
            apiSecret: apiSecret,
            memberAccount: memberAccount,
            transType: transType,
            amount: amount,
            orderNo: orderNo,
            userIP: userIP);

        Assert.Equal("2311081546291049092", actual.APIOrderNo);
        Assert.Equal(999.0m, actual.Balance);
        Assert.Equal(response.ToJsonString(), actual.Resp);
    }

    [Fact]
    public async Task Duplicate_deposit_and_withdrawal_transaction_number()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var memberAccount = "4ue0ujotpmuu";
        var userIP = "127.0.0.1";
        var transType = TransType.Withdraw;
        var amount = 1.0m;
        var orderNo = "bb1b2e47fb3a4fab97ec00deb731f9fa";

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

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699429332);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetTransferHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            memberAccount: Arg.Is(memberAccount),
            memberPassword: Arg.Is(gameApiSettingsOptions.Value.MemberPassword),
            orderNo: Arg.Is(orderNo),
            transType: Arg.Is(transType.ToString()),
            amount: Arg.Is(amount),
            apiSecret: Arg.Is(apiSecret),
            timestamp: Arg.Is(currentTime.ToUnixTimeSeconds()))
            .Returns(hash);

        var request = new JsonObject
        {
            ["ApiKey"] = apiKey,
            ["Timestamp"] = currentTime.ToUnixTimeSeconds(),
            ["Game"] = gameApiSettingsOptions.Value.Game,
            ["MemberAccount"] = memberAccount,
            ["MemberPassword"] = gameApiSettingsOptions.Value.MemberPassword,
            ["TransType"] = transType.ToString(),
            ["Amount"] = amount,
            ["OrderNo"] = orderNo,
            ["UserIP"] = userIP,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = false,
            ["Code"] = 1005,
            ["Message"] = "存提款交易编号重複",
            ["Data"] = new JsonObject
            {
                ["APIOrderNo"] = null,
                ["Balance"] = 0.0
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/Transfer").ToString())
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

        _ = await Assert.ThrowsAsync<ReturnCodeException>(
            () => client.TransferAsync(
                apiUrl: apiUrl,
                apiKey: apiKey,
                apiSecret: apiSecret,
                memberAccount: memberAccount,
                transType: transType,
                amount: amount,
                orderNo: orderNo,
                userIP: userIP)
            .AsTask());
    }
}
