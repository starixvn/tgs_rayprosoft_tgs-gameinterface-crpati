namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public record FundOutQuery(
    Uri ApiUrl,
    string ApiKey,
    string ApiSecret,
    string PlayerCode,
    string IpAddress,
    string OrderId,
    decimal Amount);
