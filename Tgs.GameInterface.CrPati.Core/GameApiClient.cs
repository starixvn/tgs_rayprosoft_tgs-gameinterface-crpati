using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameInterface.CrPati.Core.Exceptions;
using Tgs.GameInterface.CrPati.Core.Extensions;
using Tgs.GameInterface.CrPati.Core.Models;

namespace Tgs.GameInterface.CrPati.Core;

internal class GameApiClient(
	ILogger<GameApiClient> logger,
	IOptions<GameApiSettingsOptions> gameApiSettingsOptions,
	IOptions<PolicySettingsOptions> policySettingsOptions,
	ISystemClock systemClock,
	IHashCryptography cryptography,
	HttpClient client) : IGameApiClient
{
	private readonly GameApiSettingsOptions _gameApiSettings = gameApiSettingsOptions.Value;
	private readonly PolicySettingsOptions _policySettings = policySettingsOptions.Value;

	public async ValueTask<BalanceResult> BalanceAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, string userIP, CancellationToken cancellationToken = default)
	{
		var timestamp = systemClock.CurrentTime.ToUnixTimeSeconds();
		var hash = cryptography.GetBalanceHash(apiKey, _gameApiSettings.Game, memberAccount, _gameApiSettings.MemberPassword, apiSecret, timestamp);
		var json = new JsonObject
		{
			["ApiKey"] = apiKey,
			["Timestamp"] = timestamp,
			["Game"] = _gameApiSettings.Game,
			["MemberAccount"] = memberAccount,
			["MemberPassword"] = _gameApiSettings.MemberPassword,
			["UserIP"] = userIP,
			["Hash"] = hash
		};

		var jsonNode = await PostAsync(
			apiUrl: apiUrl,
			path: "/Api/Game/Balance",
			jsonNode: json,
			logPrefix: "Balance",
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !jsonNode["IsSuccess"]!.GetValue<bool>()
			? jsonNode["Code"]!.GetValue<int>() == 2008
				? new BalanceResult(false, 0)
				: throw new ReturnCodeException(jsonNode["Code"]!.GetValue<int>())
			: new BalanceResult(true, jsonNode["Data"]!["Balance"]!.GetValue<decimal>());
	}

	public async IAsyncEnumerable<BetRecordResult> BetRecordByPageAsyncc(Uri apiUrl, string apiKey, string apiSecret, DateTimeOffset startDate, DateTimeOffset endDate, int pageNumber = 1, int pageSize = 1000, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var timestamp = systemClock.CurrentTime.ToUnixTimeSeconds();
		var hash = cryptography.GetBetRecordByPageHash(apiKey, _gameApiSettings.Game, startDate.DatetimeConvertToString(), endDate.DatetimeConvertToString(), apiSecret, timestamp);
		var json = new JsonObject
		{
			["ApiKey"] = apiKey,
			["Timestamp"] = timestamp,
			["Game"] = _gameApiSettings.Game,
			["StartDate"] = startDate.DatetimeConvertToString(),
			["EndDate"] = endDate.DatetimeConvertToString(),
			["PageNumber"] = pageNumber,
			["PageSize"] = pageSize,
			["Hash"] = hash
		};

		var retryPolicy = Policy
			.HandleResult<JsonNode>(jsonNode => !jsonNode["IsSuccess"]!.GetValue<bool>() && jsonNode["Code"]!.GetValue<int>() == 2019)
			.WaitAndRetryAsync(
			retryCount: _policySettings.BetRecordByPageRetryCount,
			retryAttempt => TimeSpan.FromSeconds(_policySettings.BetRecordByPageRetryWaitSec),
			onRetry: (response, retryTime, context) =>
			{
				logger.LogWarning("LogOn:{logOn} | Message:拉單過於頻繁(兩次拉單時間間隔必須大於 10 秒 | Json:{jsonNode}", DateTime.Now.ToString("o"), response.Result.ToJsonString());
				timestamp = systemClock.CurrentTime.ToUnixTimeSeconds();
				hash = cryptography.GetBetRecordByPageHash(apiKey, _gameApiSettings.Game, startDate.DatetimeConvertToString(), endDate.DatetimeConvertToString(), apiSecret, timestamp);
				json = new JsonObject
				{
					["ApiKey"] = apiKey,
					["Timestamp"] = timestamp,
					["Game"] = _gameApiSettings.Game,
					["StartDate"] = startDate.DatetimeConvertToString(),
					["EndDate"] = endDate.DatetimeConvertToString(),
					["PageNumber"] = pageNumber,
					["PageSize"] = pageSize,
					["Hash"] = hash
				};
			});

		var jsonNode = await retryPolicy.ExecuteAsync(
			() => PostAsync(
				apiUrl: apiUrl,
				path: "/Api/Game/BetRecordByPage",
				jsonNode: json,
				logPrefix: "BetRecordByPage",
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		if (!jsonNode["IsSuccess"]!.GetValue<bool>())
		{
			throw new ReturnCodeException(jsonNode["Code"]!.GetValue<int>());
		}

		var datas = jsonNode["Data"]?.AsArray();

		if (datas is null || datas.Count == 0)
		{
			yield break;
		}

		foreach (var data in datas)
		{
			if (data is null)
			{
				continue;
			}

			yield return new BetRecordResult(
				BetID: data["BetID"]!.GetValue<string>(),
				Odds: data["Odds"]!.GetValue<decimal>(),
				OddsType: data["OddsType"]!.GetValue<int>(),
				GameCode: data["GameCode"]!.GetValue<string>(),
				MemberAccount: data["MemberAccount"]!.GetValue<string>(),
				ModifyDate: data["ModifyDate"]!.GetValue<DateTimeOffset>(),
				BetDate: data["BetDate"]!.GetValue<DateTimeOffset>(),
				SettleDate: data["SettleDate"]?.GetValue<DateTimeOffset>() ?? DateTimeOffset.UnixEpoch,
				Bet: data["Bet"]!.GetValue<decimal>(),
				ValidBet: data["ValidBet"]!.GetValue<decimal>(),
				TotalPayout: data["TotalPayout"]!.GetValue<decimal>(),
				TotalWinlose: data["TotalWinlose"]!.GetValue<decimal>(),
				BetContent: data["BetContent"]!.GetValue<string>(),
				//結算狀態 (0:未結算 1:取消/退回 2: 拒絕 3:已結算 4:提前結算/兌現)
				BetStatus: data["BetStatus"]!.GetValue<int>(),
				GameType: data["GameType"]!.GetValue<int>(),
				GameKind: data["GameKind"]!.GetValue<int>(),
				AdditionalData: data.ToJsonString());
		}
	}

	public async ValueTask<bool> CheckTransferAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, string orderNo, string userIP, CancellationToken cancellationToken = default)
	{
		var timestamp = systemClock.CurrentTime.ToUnixTimeSeconds();
		var hash = cryptography.GetCheckTransferHash(apiKey, _gameApiSettings.Game, memberAccount, _gameApiSettings.MemberPassword, orderNo, apiSecret, timestamp);
		var json = new JsonObject
		{
			["ApiKey"] = apiKey,
			["Timestamp"] = timestamp,
			["Game"] = _gameApiSettings.Game,
			["MemberAccount"] = memberAccount,
			["MemberPassword"] = _gameApiSettings.MemberPassword,
			["OrderNo"] = orderNo,
			["UserIP"] = userIP,
			["Hash"] = hash
		};

		var jsonNode = await PostAsync(
			apiUrl: apiUrl,
			path: "/Api/Game/CheckTransfer",
			jsonNode: json,
			logPrefix: "CheckTransfer",
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return jsonNode["IsSuccess"]!.GetValue<bool>()
			|| (jsonNode["Code"]!.GetValue<int>() == 2022 ? false
			: throw new ReturnCodeException(jsonNode["Code"]!.GetValue<int>()));
	}

	public async ValueTask<LoginResult> LoginAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, string userIP, int languageCode, bool isMobile, bool isTrial, CancellationToken cancellationToken = default)
	{
		var timestamp = systemClock.CurrentTime.ToUnixTimeSeconds();
		var hash = cryptography.GetLoginHash(apiKey, _gameApiSettings.Game, memberAccount, _gameApiSettings.MemberPassword, _gameApiSettings.GameCode, userIP, apiSecret, timestamp);
		var json = new JsonObject
		{
			["ApiKey"] = apiKey,
			["Timestamp"] = timestamp,
			["Game"] = _gameApiSettings.Game,
			["MemberAccount"] = memberAccount,
			["MemberPassword"] = _gameApiSettings.MemberPassword,
			["GameCode"] = _gameApiSettings.GameCode,
			["UserIP"] = userIP,
			["DeviceType"] = isMobile ? 1 : 0,
			["LanguageCode"] = languageCode,
			["IsTrial"] = isTrial ? 1 : 0,
			["Hash"] = hash
		};

		var jsonNode = await PostAsync(
			apiUrl: apiUrl,
			path: "/Api/Game/Login",
			jsonNode: json,
			logPrefix: "Login",
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !jsonNode["IsSuccess"]!.GetValue<bool>()
			? jsonNode["Code"]!.GetValue<int>() == 2008
				? new LoginResult(false, string.Empty)
				: throw new ReturnCodeException(jsonNode["Code"]!.GetValue<int>())
			: new LoginResult(true, jsonNode["Data"]!["GameUrl"]!.GetValue<string>());
	}

	public async ValueTask RegisterAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, string userIP, CancellationToken cancellationToken = default)
	{
		var timestamp = systemClock.CurrentTime.ToUnixTimeSeconds();
		var hash = cryptography.GetRegisterHash(apiKey, _gameApiSettings.Game, memberAccount, _gameApiSettings.MemberPassword, apiSecret, timestamp);
		var json = new JsonObject
		{
			["ApiKey"] = apiKey,
			["Timestamp"] = timestamp,
			["Game"] = _gameApiSettings.Game,
			["MemberAccount"] = memberAccount,
			["MemberPassword"] = _gameApiSettings.MemberPassword,
			["UserIP"] = userIP,
			["Hash"] = hash
		};

		var jsonNode = await PostAsync(
			apiUrl: apiUrl,
			path: "/Api/Game/Register",
			jsonNode: json,
			logPrefix: "Register",
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!jsonNode["IsSuccess"]!.GetValue<bool>())
		{
			if (jsonNode["Code"]!.GetValue<int>() != 2002)
			{
				throw new ReturnCodeException(jsonNode["Code"]!.GetValue<int>());
			}
		}
	}

	public async ValueTask<TransferResult> TransferAsync(Uri apiUrl, string apiKey, string apiSecret, string memberAccount, TransType transType, decimal amount, string orderNo, string userIP, CancellationToken cancellationToken = default)
	{
		var timestamp = systemClock.CurrentTime.ToUnixTimeSeconds();
		var hash = cryptography.GetTransferHash(apiKey, _gameApiSettings.Game, memberAccount, _gameApiSettings.MemberPassword, orderNo, transType.ToString(), amount, apiSecret, timestamp);
		var json = new JsonObject
		{
			["ApiKey"] = apiKey,
			["Timestamp"] = timestamp,
			["Game"] = _gameApiSettings.Game,
			["MemberAccount"] = memberAccount,
			["MemberPassword"] = _gameApiSettings.MemberPassword,
			["TransType"] = transType.ToString(),
			["Amount"] = amount,
			["OrderNo"] = orderNo,
			["UserIP"] = userIP,
			["Hash"] = hash
		};

		var jsonNode = await PostAsync(
			apiUrl: apiUrl,
			path: "/Api/Game/Transfer",
			jsonNode: json,
			logPrefix: "Transfer",
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !jsonNode["IsSuccess"]!.GetValue<bool>()
			? throw new ReturnCodeException(jsonNode["Code"]!.GetValue<int>())
			: new TransferResult(
			jsonNode["Data"]!["APIOrderNo"]!.ToString(),
			jsonNode["Data"]!["Balance"]!.GetValue<decimal>(),
			jsonNode.ToJsonString());
	}

	private async ValueTask<JsonNode> PostAsync(Uri apiUrl, string path, JsonNode jsonNode, string logPrefix, CancellationToken cancellationToken = default)
	{
		var uriBuilder = new UriBuilder(new Uri(apiUrl, path));

		logPrefix = $"{nameof(GameApiClient)}.{logPrefix}";

		var requestContent = new StringContent(jsonNode.ToJsonString(), Encoding.UTF8, mediaType: MediaTypeNames.Application.Json);

		var sw = Stopwatch.StartNew();

		var httpResponseMessage = await client.PostAsync(
			requestUri: uriBuilder.ToString(),
			content: requestContent,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		sw.Stop();

		var responseMessage = default(string);
		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr.ReadToEndAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? string.Empty;
		}

		var logEvent = new GameApiLogEvent
		{
			Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix ?? throw new ArgumentNullException(nameof(logPrefix)),
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
			RequestContent = (await requestContent.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
			ResponseHeaders = httpResponseMessage.Headers.ToString(),
			ResponseContent = responseMessage.NoLongerThan(4096),
			ResponseCode = (short)httpResponseMessage.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};

		logger.LogInformation("{logEvent}", JsonSerializer.Serialize(logEvent));

		_ = httpResponseMessage.EnsureSuccessStatusCode();

		return string.IsNullOrEmpty(responseMessage)
			? throw new Exception("Game provider response content is empty.")
			: JsonNode.Parse(responseMessage)!;
	}
}
