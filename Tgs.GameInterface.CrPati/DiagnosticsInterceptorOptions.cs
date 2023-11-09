using System.Diagnostics;

namespace Tgs.GameInterface.CrPati;

public class DiagnosticsInterceptorOptions
{
    public ActivitySource InterceptorActivitySource { get; set; } =
        new ActivitySource($"Tgs.GameInterface.CrPati.DiagnosticsInterceptor");
}
