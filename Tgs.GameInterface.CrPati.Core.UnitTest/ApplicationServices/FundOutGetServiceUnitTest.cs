using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CrPati.Core.ApplicationServices;
using Tgs.GameInterface.CrPati.Core.Models;

namespace Tgs.GameInterface.CrPati.Core.UnitTest.ApplicationServices;

public class FundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeClient = Substitute.For<IGameApiClient>();

		var query = new FundOutQuery(
			ApiUrl: new Uri("https://apitest.fastapi2020.com/"),
			ApiKey: "testgpi123123",
			ApiSecret: "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
			PlayerCode: "4ue0ujotpmuu",
			IpAddress: "127.0.0.1",
			OrderId: Guid.NewGuid().ToString("N"),
			Amount: 1000.0m);

		var result = new TransferResult(
			APIOrderNo: "2311081542127340855",
			Balance: 1000.0m,
			Resp: new JsonObject
			{
				["IsSuccess"] = true,
				["Code"] = 0,
				["Message"] = "执行成功",
				["Data"] = new JsonObject
				{
					["APIOrderNo"] = "2311081542127340855",
					["Balance"] = 1000.0
				}
			}.ToJsonString());
		_ = fakeClient.TransferAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			apiKey: Arg.Is(query.ApiKey),
			apiSecret: Arg.Is(query.ApiSecret),
			memberAccount: Arg.Is(query.PlayerCode),
			transType: Arg.Is(TransType.Deposit),
			amount: Arg.Is(query.Amount),
			orderNo: Arg.Is(query.OrderId),
			userIP: Arg.Is(query.IpAddress),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(result);

		var service = new FundOutGetService(
			fakeClient);

		var actual = await service.GetAsync(query: query);

		Assert.Equal("2311081542127340855", actual.APIOrderNo);
		Assert.Equal(1000.0m, actual.Balance);
		Assert.Equal(result.Resp, actual.Resp);
		;
	}
}
