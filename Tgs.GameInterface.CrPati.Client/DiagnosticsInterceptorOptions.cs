using System.Diagnostics;

namespace Tgs.GameInterface.CrPati.Client;

public class DiagnosticsInterceptorOptions
{
	public ActivitySource InterceptorActivitySource { get; set; } =
		new ActivitySource($"Tgs.GameInterface.CrPati.Client.DiagnosticsInterceptor");
}
