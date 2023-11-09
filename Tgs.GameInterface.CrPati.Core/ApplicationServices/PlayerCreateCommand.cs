namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public record PlayerCreateCommand(
    Uri ApiUrl,
    string ApiKey,
    string ApiSecret,
    string PlayerCode,
    string IpAddress);
