namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public record GameWagerResult(
	string ExternalWagerId,
	string PlayerCode,
	bool IsClosed,
	bool IsVoid,
	bool IsCancel,
	bool IsCashout,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal WinLoss,
	string AdditionalData);
