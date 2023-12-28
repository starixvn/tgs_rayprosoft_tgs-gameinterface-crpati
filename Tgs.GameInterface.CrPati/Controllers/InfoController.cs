using System.Net;
using System.Net.Sockets;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.CrPati.Core;

namespace Tgs.GameInterface.CrPati.Controllers;

[Route("[controller]")]
[ApiController]
public class InfoController(
    IOptions<GameApiSettingsOptions> options) : ControllerBase
{
    private readonly GameApiSettingsOptions _gameApiSettingsOptions = options.Value;

    [HttpGet]
    public async Task<object> GetAsync([FromServices] IWebHostEnvironment _hostingEnvironment)
    {
        var hostName = Dns.GetHostName();
        var hostEntry = await Dns.GetHostEntryAsync(hostName);
        var hostIp = Array.Find(hostEntry.AddressList,
            x => x.AddressFamily == AddressFamily.InterNetwork);

        return new
        {
            Environment.MachineName,
            HostName = hostName,
            HostIp = hostIp?.ToString() ?? string.Empty,
            Environment = _hostingEnvironment.EnvironmentName,
            OsVersion = $"{Environment.OSVersion}",
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
            ProcessCount = Environment.ProcessorCount,
            GameApiSettingsOptions = _gameApiSettingsOptions
        };
    }
}
