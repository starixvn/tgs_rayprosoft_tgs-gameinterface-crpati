namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public record GameUrlQuery(
	Uri ApiUrl,
	string ApiKey,
	string ApiSecret,
	string PlayerCode,
	string IpAddress,
	int Lang,
	bool IsMobile);
