namespace Tgs.GameInterface.CrPati.ViewModels;

public record GameWagerResultViewModel(
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
