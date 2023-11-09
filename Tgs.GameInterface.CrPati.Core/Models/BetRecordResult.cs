namespace Tgs.GameInterface.CrPati.Core.Models;

public record BetRecordResult(
    string BetID,
    decimal Odds,
    int OddsType,
    string GameCode,
    string MemberAccount,
    DateTimeOffset ModifyDate,
    DateTimeOffset BetDate,
    DateTimeOffset SettleDate,
    decimal Bet,
    decimal ValidBet,
    decimal TotalPayout,
    decimal TotalWinlose,
    string BetContent,
    //結算狀態 (0:未結算 1:取消/退回 2: 拒絕 3:已結算 4:提前結算/兌現)
    int BetStatus,
    int GameType,
    int GameKind,
    string AdditionalData);
