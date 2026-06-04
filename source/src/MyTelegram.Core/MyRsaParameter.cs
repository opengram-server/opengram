using System.Numerics;

namespace MyTelegram.Core;

public class MyRsaParameter
{
    public BigInteger Modulus { get; set; }
    public BigInteger PrivateExponent { get; set; }
    public BigInteger PublicExponent { get; set; }
}