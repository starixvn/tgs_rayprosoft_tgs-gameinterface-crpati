namespace Tgs.GameInterface.CrPati.Core;

public interface IHashCryptography
{
	string GetLoginHash(string apiKey, string game, string memberAccount, string memberPassword, string gameCode, string userIP, string apiSecret, long timestamp);

	string GetRegisterHash(string apiKey, string game, string memberAccount, string memberPassword, string apiSecret, long timestamp);

	string GetBalanceHash(string apiKey, string game, string memberAccount, string memberPassword, string apiSecret, long timestamp);

	string GetTransferHash(string apiKey, string game, string memberAccount, string memberPassword, string orderNo, string transType, decimal amount, string apiSecret, long timestamp);

	string GetCheckTransferHash(string apiKey, string game, string memberAccount, string memberPassword, string orderNo, string apiSecret, long timestamp);

	string GetBetRecordByPageHash(string apiKey, string game, string startDate, string endDate, string apiSecret, long timestamp);
}
