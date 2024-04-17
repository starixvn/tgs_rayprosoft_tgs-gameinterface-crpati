namespace Tgs.GameInterface.CrPati.Core.Models;

public record LoginResult(
	bool IsPlayerExist,
	string GameUrl);
