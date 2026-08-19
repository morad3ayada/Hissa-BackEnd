using System.Security.Cryptography;
using System.Text;

namespace LearningPlatform.Application.Common.Helpers;

public static class OtpHasher
{
    public static string Hash(string otp)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
        return Convert.ToHexString(bytes);
    }
}
