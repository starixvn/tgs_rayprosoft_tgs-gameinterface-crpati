using NSubstitute;
using Tgs.GameInterface.CrPati.Core.ApplicationServices;
using Tgs.GameInterface.CrPati.Core.Models;

namespace Tgs.GameInterface.CrPati.Core.UnitTest.ApplicationServices;

public class GameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new GameUrlQuery(
            ApiUrl: new Uri("https://apitest.fastapi2020.com/"),
            ApiKey: "testgpi123123",
            ApiSecret: "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
            PlayerCode: "4ue0ujotpmuu",
            IpAddress: "127.0.0.1",
            Lang: 0,
            IsMobile: false);

        var result = new LoginResult(
            IsPlayerExist: true,
            GameUrl: "https://www-api.crptb.com/api/sitevalidate?DealerAccount=fastapi&UserAccount=gp4ue0ujotpmuu&PrefixCode=Fast&Token=07ab63b6-2343-44d2-823b-7f886540d691&TimeStamp=1699496899&Signature=c1c079131d5f0b79c1961b7c2d211161&Language=en-us&GameType=CP_Home");
        _ = fakeClient.LoginAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            apiKey: Arg.Is(query.ApiKey),
            apiSecret: Arg.Is(query.ApiSecret),
            memberAccount: Arg.Is(query.PlayerCode),
            userIP: Arg.Is(query.IpAddress),
            languageCode: Arg.Is(query.Lang),
            isMobile: Arg.Is(query.IsMobile),
            isTrial: Arg.Is(false),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var service = new GameUrlGetService(
            fakeClient);

        var actual = await service.GetAsync(query: query);

        Assert.True(actual.IsPlayerExist);
        Assert.Equal(result.GameUrl, actual.GameUrl);
    }
}
