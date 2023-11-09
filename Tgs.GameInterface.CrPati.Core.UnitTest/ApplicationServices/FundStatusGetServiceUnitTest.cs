using NSubstitute;
using Tgs.GameInterface.CrPati.Core.ApplicationServices;

namespace Tgs.GameInterface.CrPati.Core.UnitTest.ApplicationServices;

public class FundStatusGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new FundStatusQuery(
            ApiUrl: new Uri("https://apitest.fastapi2020.com/"),
            ApiKey: "testgpi123123",
            ApiSecret: "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
            PlayerCode: "4ue0ujotpmuu",
            IpAddress: "127.0.0.1",
            OrderId: Guid.NewGuid().ToString("N"));

        var service = new FundStatusGetService(
            fakeClient);

        _ = await service.GetAsync(query: query);

        _ = fakeClient
            .Received(1)
            .CheckTransferAsync(
            apiUrl: Arg.Is(query.ApiUrl),
            apiKey: Arg.Is(query.ApiKey),
            apiSecret: Arg.Is(query.ApiSecret),
            memberAccount: Arg.Is(query.PlayerCode),
            orderNo: Arg.Is(query.OrderId),
            userIP: Arg.Is(query.IpAddress),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
