using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.CrPati.Core.UnitTest;

public class LoginUnitTest
{
    [Fact]
    public async Task Player_does_not_exist()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var memberAccount = "4ue0ujotpmuu";
        var userIP = "127.0.0.1";
        var languageCode = 0;
        var isMobile = false;
        var isTrial = false;

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

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(1699428583);
        _ = fakeSystemClock.CurrentTime
            .Returns(currentTime);

        var hash = "test";
        _ = fakeCryptography.GetLoginHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            memberAccount: Arg.Is(memberAccount),
            memberPassword: Arg.Is(gameApiSettingsOptions.Value.MemberPassword),
            gameCode: Arg.Is(gameApiSettingsOptions.Value.GameCode),
            userIP: Arg.Is(userIP),
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
            ["GameCode"] = gameApiSettingsOptions.Value.GameCode,
            ["UserIP"] = userIP,
            ["DeviceType"] = (isMobile) ? 1 : 0,
            ["LanguageCode"] = languageCode,
            ["IsTrial"] = (isTrial) ? 1 : 0,
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
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/Login").ToString())
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

        var actual = await client.LoginAsync(
            apiUrl: apiUrl,
            apiKey: apiKey,
            apiSecret: apiSecret,
            memberAccount: memberAccount,
            userIP: userIP,
            languageCode: languageCode,
            isMobile: isMobile,
            isTrial: isTrial);

        Assert.False(actual.IsPlayerExist);
        Assert.Equal(string.Empty, actual.GameUrl);
    }

    [Fact]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("https://apitest.fastapi2020.com/");
        var apiKey = "testgpi123123";
        var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
        var memberAccount = "4ue0ujotpmuu";
        var userIP = "127.0.0.1";
        var languageCode = 0;
        var isMobile = false;
        var isTrial = false;

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
        _ = fakeCryptography.GetLoginHash(
            apiKey: Arg.Is(apiKey),
            game: Arg.Is(gameApiSettingsOptions.Value.Game),
            memberAccount: Arg.Is(memberAccount),
            memberPassword: Arg.Is(gameApiSettingsOptions.Value.MemberPassword),
            gameCode: Arg.Is(gameApiSettingsOptions.Value.GameCode),
            userIP: Arg.Is(userIP),
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
            ["GameCode"] = gameApiSettingsOptions.Value.GameCode,
            ["UserIP"] = userIP,
            ["DeviceType"] = 0,
            ["LanguageCode"] = languageCode,
            ["IsTrial"] = 0,
            ["Hash"] = hash
        };
        var response = new JsonObject
        {
            ["IsSuccess"] = true,
            ["Code"] = 0,
            ["Message"] = "执行成功",
            ["Data"] = new JsonObject
            {
                ["GameUrl"] = "https://www-api.crptb.com/api/sitevalidate?DealerAccount=fastapi&UserAccount=gp4ue0ujotpmuu&PrefixCode=Fast&Token=07ab63b6-2343-44d2-823b-7f886540d691&TimeStamp=1699496899&Signature=c1c079131d5f0b79c1961b7c2d211161&Language=en-us&GameType=CP_Home"
            }
        };

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/Login").ToString())
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

        var actual = await client.LoginAsync(
            apiUrl: apiUrl,
            apiKey: apiKey,
            apiSecret: apiSecret,
            memberAccount: memberAccount,
            userIP: userIP,
            languageCode: languageCode,
            isMobile: isMobile,
            isTrial: isTrial);

        Assert.True(actual.IsPlayerExist);
        Assert.True(actual.GameUrl.Contains("https://www-api.crptb.com/", StringComparison.InvariantCultureIgnoreCase));
    }
}
