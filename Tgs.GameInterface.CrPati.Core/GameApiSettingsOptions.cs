namespace Tgs.GameInterface.CrPati.Core;

public record GameApiSettingsOptions
{
    public string Game { get; set; } = "CP";
    public string GameCode { get; set; } = "CPCP_Home";
    public string MemberPassword { get; set; } = "TgsPassw0rd";
}
