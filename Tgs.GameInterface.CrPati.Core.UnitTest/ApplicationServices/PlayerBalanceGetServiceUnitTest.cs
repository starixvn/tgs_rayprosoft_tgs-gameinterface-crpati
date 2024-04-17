using NSubstitute;
using Tgs.GameInterface.CrPati.Core.ApplicationServices;
using Tgs.GameInterface.CrPati.Core.Models;

namespace Tgs.GameInterface.CrPati.Core.UnitTest.ApplicationServices;

public class PlayerBalanceGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeClient = Substitute.For<IGameApiClient>();

		var query = new PlayerBalanceQuery(
			ApiUrl: new Uri("https://apitest.fastapi2020.com/"),
			ApiKey: "testgpi123123",
			ApiSecret: "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
			PlayerCode: "4ue0ujotpmuu",
			IpAddress: "127.0.0.1");

		var result = new BalanceResult(
			IsPlayerExist: true,
			Balance: 999.0m);
		_ = fakeClient.BalanceAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiSecret: Arg.Is(query.ApiSecret),
			memberAccount: Arg.Is(query.PlayerCode),
			userIP: Arg.Is(query.IpAddress),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(result);

		var service = new PlayerBalanceGetService(
			fakeClient);

		var actual = await service.GetAsync(query: query);

		Assert.True(actual.IsPlayerExist);
		Assert.Equal(999.0m, actual.Balance);
	}
}
