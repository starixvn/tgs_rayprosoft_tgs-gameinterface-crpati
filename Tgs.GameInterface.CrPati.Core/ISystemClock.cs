namespace Tgs.GameInterface.CrPati.Core;

public interface ISystemClock
{
	DateTimeOffset CurrentTime { get; }
}
