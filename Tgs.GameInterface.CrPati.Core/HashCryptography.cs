using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Tgs.GameInterface.CrPati.Core;

internal class HashCryptography(
    ILogger<HashCryptography> logger) : IHashCryptography
{
    public string GetBalanceHash(string apiKey, string game, string memberAccount, string memberPassword, string apiSecret, long timestamp)
    {
        var stringBuilder = new StringBuilder();
        _ = stringBuilder.Append(apiKey);
        _ = stringBuilder.Append(game);
        _ = stringBuilder.Append(memberAccount);
        _ = stringBuilder.Append(memberPassword);
        _ = stringBuilder.Append(apiSecret);
        _ = stringBuilder.Append(timestamp);

        var hash = GetHash(stringBuilder.ToString());

        logger.LogDebug("LogOn:{logOn} | data:{data} | hash:{hash}", DateTime.Now.ToString("o"), stringBuilder.ToString(), hash);

        return hash;
    }

    public string GetBetRecordByPageHash(string apiKey, string game, string startDate, string endDate, string apiSecret, long timestamp)
    {
        var stringBuilder = new StringBuilder();
        _ = stringBuilder.Append(apiKey);
        _ = stringBuilder.Append(game);
        _ = stringBuilder.Append(startDate);
        _ = stringBuilder.Append(endDate);
        _ = stringBuilder.Append(apiSecret);
        _ = stringBuilder.Append(timestamp);

        var hash = GetHash(stringBuilder.ToString());

        logger.LogDebug("LogOn:{logOn} | data:{data} | hash:{hash}", DateTime.Now.ToString("o"), stringBuilder.ToString(), hash);

        return hash;
    }

    public string GetCheckTransferHash(string apiKey, string game, string memberAccount, string memberPassword, string orderNo, string apiSecret, long timestamp)
    {
        var stringBuilder = new StringBuilder();
        _ = stringBuilder.Append(apiKey);
        _ = stringBuilder.Append(game);
        _ = stringBuilder.Append(memberAccount);
        _ = stringBuilder.Append(memberPassword);
        _ = stringBuilder.Append(orderNo);
        _ = stringBuilder.Append(apiSecret);
        _ = stringBuilder.Append(timestamp);

        var hash = GetHash(stringBuilder.ToString());

        logger.LogDebug("LogOn:{logOn} | data:{data} | hash:{hash}", DateTime.Now.ToString("o"), stringBuilder.ToString(), hash);

        return hash;
    }

    public string GetLoginHash(string apiKey, string game, string memberAccount, string memberPassword, string gameCode, string userIP, string apiSecret, long timestamp)
    {
        var stringBuilder = new StringBuilder();
        _ = stringBuilder.Append(apiKey);
        _ = stringBuilder.Append(game);
        _ = stringBuilder.Append(memberAccount);
        _ = stringBuilder.Append(memberPassword);
        _ = stringBuilder.Append(gameCode);
        _ = stringBuilder.Append(userIP);
        _ = stringBuilder.Append(apiSecret);
        _ = stringBuilder.Append(timestamp);

        var hash = GetHash(stringBuilder.ToString());

        logger.LogDebug("LogOn:{logOn} | data:{data} | hash:{hash}", DateTime.Now.ToString("o"), stringBuilder.ToString(), hash);

        return hash;
    }

    public string GetRegisterHash(string apiKey, string game, string memberAccount, string memberPassword, string apiSecret, long timestamp)
    {
        var stringBuilder = new StringBuilder();
        _ = stringBuilder.Append(apiKey);
        _ = stringBuilder.Append(game);
        _ = stringBuilder.Append(memberAccount);
        _ = stringBuilder.Append(memberPassword);
        _ = stringBuilder.Append(apiSecret);
        _ = stringBuilder.Append(timestamp);

        var hash = GetHash(stringBuilder.ToString());

        logger.LogDebug("LogOn:{logOn} | data:{data} | hash:{hash}", DateTime.Now.ToString("o"), stringBuilder.ToString(), hash);

        return hash;
    }

    public string GetTransferHash(string apiKey, string game, string memberAccount, string memberPassword, string orderNo, string transType, decimal amount, string apiSecret, long timestamp)
    {
        var stringBuilder = new StringBuilder();
        _ = stringBuilder.Append(apiKey);
        _ = stringBuilder.Append(game);
        _ = stringBuilder.Append(memberAccount);
        _ = stringBuilder.Append(memberPassword);
        _ = stringBuilder.Append(orderNo);
        _ = stringBuilder.Append(transType);
        _ = stringBuilder.Append(amount);
        _ = stringBuilder.Append(apiSecret);
        _ = stringBuilder.Append(timestamp);

        var hash = GetHash(stringBuilder.ToString());

        logger.LogDebug("LogOn:{logOn} | data:{data} | hash:{hash}", DateTime.Now.ToString("o"), stringBuilder.ToString(), hash);

        return hash;
    }

    private static string GetHash(string data)

    {
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var md5 = MD5.Create();
        using var memoryStream = new MemoryStream(dataBytes);
        var bytes = md5.ComputeHash(inputStream: memoryStream);
        var stringBuilder = new StringBuilder();
        foreach (var b in bytes)
        {
            _ = stringBuilder.Append(b.ToString("x").PadLeft(2, '0'));
        }

        return stringBuilder.ToString();
    }
}
