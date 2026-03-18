using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class AESCrypto
{
    private static readonly string Key = "EscapePlanetKey!EscapePlanetKey!"; // 32 chars = AES-256

    public static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.GenerateIV();
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                    sw.Write(plainText);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string Decrypt(string cipherText)
    {
        byte[] full = Convert.FromBase64String(cipherText);
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(Key);
            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipher = new byte[full.Length - iv.Length];
            Array.Copy(full, iv, iv.Length);
            Array.Copy(full, iv.Length, cipher, 0, cipher.Length);
            aes.IV = iv;
            using (var ms = new MemoryStream(cipher))
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
                return sr.ReadToEnd();
        }
    }
}
