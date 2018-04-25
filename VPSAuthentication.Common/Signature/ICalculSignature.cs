namespace VPSAuthentication.Common.Signature
{
    public interface ICalculSignature
    {
        string Signature(string secret, string value);
    }
}
