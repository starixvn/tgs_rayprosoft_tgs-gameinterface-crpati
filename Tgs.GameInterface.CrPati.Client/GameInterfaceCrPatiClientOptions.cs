namespace Tgs.GameInterface.CrPati.Client;

public class GameInterfaceCrPatiClientOptions
{
	public Uri BaseUrl { get; set; } = default!;
	public TimeSpan GrpcConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);
}
