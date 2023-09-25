using System.Runtime.InteropServices;
using System.Security;

namespace APIPointeuse.Models
{
    public static class TokenManager
    {
        public static SecureString Token { get; private set; }

        public static void SetToken(SecureString token)
        {
            Token = token;
        }

        public static string ConvertSecureStringToString(SecureString secureString)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }
    }
}
