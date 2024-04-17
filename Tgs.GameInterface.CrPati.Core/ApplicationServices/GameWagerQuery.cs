namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public record GameWagerQuery(
	Uri ApiUrl,
	string ApiKey,
	string ApiSecret,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime,
	int PageNumber = 1,
	int PageSize = 1000);
