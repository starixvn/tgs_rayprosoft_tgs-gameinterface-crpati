namespace Tgs.GameInterface.CrPati.Core;

public record PolicySettingsOptions
{
	/// <summary>
	/// The bet record by page retry count
	/// </summary>
	public int BetRecordByPageRetryCount { get; set; }
	/// <summary>
	/// The bet record by page retry wait sec
	/// </summary>
	public int BetRecordByPageRetryWaitSec { get; set; }
}
