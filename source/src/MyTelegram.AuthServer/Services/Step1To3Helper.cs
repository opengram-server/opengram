using System.Runtime.CompilerServices;

namespace MyTelegram.AuthServer.Services;

public class Step1To3Helper
{
    private static readonly BigInteger TwoPowOf2048Sub64 = BigInteger.Pow(2, 2048 - 64);

    protected void CheckRequestData(
        Span<byte> expected,
        Span<byte> actual,
        [CallerArgumentExpression(nameof(actual))]
        string? message = null
    )
    {
        if (!expected.SequenceEqual(actual))
        {
            throw new ArgumentException(
                $"Invalid {message}, expected: {expected.ToHexString()} actual: {actual.ToHexString()}"
            );
        }
    }

    protected string GetAuthCacheKey(byte[] serverNonce)
    {
        return AuthCacheItem.GetCacheKey(serverNonce);
    }

    /// <summary>
    ///     https://corefork.telegram.org/mtproto/auth_key#6-server-responds-with
    /// </summary>
    /// <param name="gaOrGb"></param>
    /// <param name="dhPrime"></param>
    /// <returns></returns>
    protected bool IsGoodGaOrGb(BigInteger gaOrGb, BigInteger dhPrime)
    {
        var dhPrimeSubTowPowOf2048Sub64 = dhPrime - TwoPowOf2048Sub64;
        var isGoodGaOrGb =
            gaOrGb > 1
            && gaOrGb < dhPrime - 1
            && gaOrGb > TwoPowOf2048Sub64
            && gaOrGb < dhPrimeSubTowPowOf2048Sub64;

        return isGoodGaOrGb;
    }
}