using Microsoft.Extensions.Logging.Abstractions;

namespace Tgs.GameInterface.CrPati.Core.UnitTest;

public class HashCryptographyUnitTest
{
    [Fact]
    public void Get_Balance_Hash()
    {
        var fakeLog = NullLogger<HashCryptography>.Instance;
        var cryptography = new HashCryptography(fakeLog);
        var hash = cryptography.GetBalanceHash("testgpi123123", "CP", "4ue0ujotpmuu", "TgsPassw0rd", "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR", 1699423023);
        Assert.Equal("f18257e8b9b47741057f9886a78a904c", hash);
    }

    [Fact]
    public void Get_Transfer_Hash_Withdraw()
    {
        var fakeLog = NullLogger<HashCryptography>.Instance;
        var cryptography = new HashCryptography(fakeLog);
        var hash = cryptography.GetTransferHash("testgpi123123", "CP", "4ue0ujotpmuu", "TgsPassw0rd", "bb1b2e47fb3a4fab97ec00deb731f9fa", "Withdraw", 1, "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR", 1699423083);
        Assert.Equal("8b751b63e12bbc7b1f87963e9a0b8d41", hash);
    }

    [Fact]
    public void Get_Transfer_Hash_Deposit()
    {
        var fakeLog = NullLogger<HashCryptography>.Instance;
        var cryptography = new HashCryptography(fakeLog);
        var hash = cryptography.GetTransferHash("testgpi123123", "CP", "4ue0ujotpmuu", "TgsPassw0rd", "bb1b2e47fb3a4fab97ec00deb731f9fa", "Deposit", 1, "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR", 1699423115);
        Assert.Equal("4edc75ea570709ce1123c02b47cc57eb", hash);
    }

    [Fact]
    public void Get_CheckTransfer_Hash()
    {
        var fakeLog = NullLogger<HashCryptography>.Instance;
        var cryptography = new HashCryptography(fakeLog);
        var hash = cryptography.GetCheckTransferHash("testgpi123123", "CP", "4ue0ujotpmuu", "TgsPassw0rd", "a07fb0047df74ced9ba07c93ba152458", "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR", 1699423145);
        Assert.Equal("301b27ea46ab1352e4689a650fccab3f", hash);
    }

    [Fact]
    public void Get_Login_Hash()
    {
        var fakeLog = NullLogger<HashCryptography>.Instance;
        var cryptography = new HashCryptography(fakeLog);
        var hash = cryptography.GetLoginHash("testgpi123123", "CP", "4ue0ujotpmuu", "TgsPassw0rd", "CPCP_Home", "127.0.0.1", "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR", 1699423198);
        Assert.Equal("e495f5dc1dd1a02bca5c1cdeee033d8f", hash);
    }

    [Fact]
    public void Get_BetRecordByPage_Hash()
    {
        var fakeLog = NullLogger<HashCryptography>.Instance;
        var cryptography = new HashCryptography(fakeLog);
        var hash = cryptography.GetBetRecordByPageHash("testgpi123123", "CP", "2023-11-08 17:00:00", "2023-11-08 18:00:00", "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR", 1699434995);
        Assert.Equal("a53ff011c101f0c15ffa4fc7858f7c25", hash);
    }
}
