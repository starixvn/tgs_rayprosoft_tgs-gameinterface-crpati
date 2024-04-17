using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.CrPati.Core.UnitTest;

public class RegisterUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://apitest.fastapi2020.com/");
		var apiKey = "testgpi123123";
		var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
		var memberAccount = "4ue0ujotpmuu";
		var userIP = "127.0.0.1";

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
		_ = fakeCryptography.GetRegisterHash(
			apiKey: Arg.Is(apiKey),
			game: Arg.Is(gameApiSettingsOptions.Value.Game),
			memberAccount: Arg.Is(memberAccount),
			memberPassword: Arg.Is(gameApiSettingsOptions.Value.MemberPassword),
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
			["UserIP"] = userIP,
			["Hash"] = hash
		};
		var response = new JsonObject
		{
			["IsSuccess"] = true,
			["Code"] = 0,
			["Message"] = "执行成功",
			["Data"] = null
		};

		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/Register").ToString())
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

		await client.RegisterAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiSecret: apiSecret,
			memberAccount: memberAccount,
			userIP: userIP);
	}

	[Fact]
	public async Task Player_Repeats()
	{
		var apiUrl = new Uri("https://apitest.fastapi2020.com/");
		var apiKey = "testgpi123123";
		var apiSecret = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR";
		var memberAccount = "4ue0ujotpmuu";
		var userIP = "127.0.0.1";

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
		_ = fakeCryptography.GetRegisterHash(
			apiKey: Arg.Is(apiKey),
			game: Arg.Is(gameApiSettingsOptions.Value.Game),
			memberAccount: Arg.Is(memberAccount),
			memberPassword: Arg.Is(gameApiSettingsOptions.Value.MemberPassword),
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
			["UserIP"] = userIP,
			["Hash"] = hash
		};
		var response = new JsonObject
		{
			["IsSuccess"] = false,
			["Code"] = 2002,
			["Message"] = "会员帐号重複",
			["Data"] = null
		};

		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/Api/Game/Register").ToString())
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

		await client.RegisterAsync(
			apiUrl: apiUrl,
			apiKey: apiKey,
			apiSecret: apiSecret,
			memberAccount: memberAccount,
			userIP: userIP);
	}
}
