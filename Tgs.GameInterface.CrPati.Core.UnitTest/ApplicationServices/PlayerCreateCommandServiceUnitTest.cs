using NSubstitute;
using Tgs.GameInterface.CrPati.Core.ApplicationServices;

namespace Tgs.GameInterface.CrPati.Core.UnitTest.ApplicationServices;

public class PlayerCreateCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var command = new PlayerCreateCommand(
            ApiUrl: new Uri("https://apitest.fastapi2020.com/"),
            ApiKey: "testgpi123123",
            ApiSecret: "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
            PlayerCode: "4ue0ujotpmuu",
            IpAddress: "127.0.0.1");

        var service = new PlayerCreateCommandService(
            fakeClient);

        await service.ExecuteAsync(command: command);

        _ = fakeClient
            .Received(1)
            .RegisterAsync(
            apiUrl: Arg.Is(command.ApiUrl),
            apiKey: Arg.Is(command.ApiKey),
            apiSecret: Arg.Is(command.ApiSecret),
            memberAccount: Arg.Is(command.PlayerCode),
            userIP: Arg.Is(command.IpAddress),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
