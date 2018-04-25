namespace VPSAuthentication.Common.Signature
{
    public interface ICalculateSignature
    {
        string Signature(string key, string value);
    }
}
