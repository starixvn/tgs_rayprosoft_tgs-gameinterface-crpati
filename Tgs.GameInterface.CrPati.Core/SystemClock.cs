namespace Tgs.GameInterface.CrPati.Core;

internal class SystemClock : ISystemClock
{
	public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;
}
