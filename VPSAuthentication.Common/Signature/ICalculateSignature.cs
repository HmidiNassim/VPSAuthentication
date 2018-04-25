namespace VPSAuthentication.Common.Signature
{
    public interface ICalculateSignature
    {
        string Signature(string secret, string value);
    }
}
