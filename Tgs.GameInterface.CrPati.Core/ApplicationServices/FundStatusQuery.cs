namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public record FundStatusQuery(
    Uri ApiUrl,
    string ApiKey,
    string ApiSecret,
    string PlayerCode,
    string IpAddress,
    string OrderId);
