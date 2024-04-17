namespace Tgs.GameInterface.CrPati.Core.ApplicationServices;

public record FundResult(
	string APIOrderNo,
	decimal Balance,
	string Resp);
