namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public record PlayerBalanceQuery(
	Uri ApiUrl,
	string ApiKey,
	string ApiSecret,
	string PlayerCode,
	string IpAddress);
