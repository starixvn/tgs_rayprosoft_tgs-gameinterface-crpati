using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

internal class PlayerCreateCommandService(IGameApiClient client) : ICommandService<PlayerCreateCommand>
{
	public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
		=> client.RegisterAsync(
			apiUrl: command.ApiUrl,
			apiKey: command.ApiKey,
			apiSecret: command.ApiSecret,
			memberAccount: command.PlayerCode,
			userIP: command.IpAddress,
			cancellationToken: cancellationToken);
}
