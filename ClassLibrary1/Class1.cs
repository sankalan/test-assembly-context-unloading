using Common;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;

namespace ClassLibrary1
{
    [Special]
    public class Class1
    {
        public void Do()
        {
            byte[] s_additionalEntropy = { 9, 8, 7, 6, 5 };
            byte[] secret = { 0, 1, 2, 3, 4, 1, 2, 3, 4 };
            ProtectedData.Protect(secret, s_additionalEntropy, DataProtectionScope.CurrentUser);
            Console.WriteLine("Done.");
        }
    }
}
