using System;
using System.Security.Cryptography;
using System.Text;

public class HighScoreCipher
{
    private const int IvLength = 16;
    private const int MacLength = 32;
    private const int AesBlockLength = 16;

    private readonly byte[] encryptionKey;
    private readonly byte[] macKey;

    public HighScoreCipher(string secret)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            encryptionKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret + ":encryption"));
            macKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret + ":mac"));
        }
    }

    public byte[] Encrypt(int score)
    {
        byte[] plainBytes = BitConverter.GetBytes(score);

        using (Aes aes = CreateAes())
        {
            aes.GenerateIV();
            byte[] cipherBytes;
            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }

            int signedLength = IvLength + cipherBytes.Length;
            byte[] fileBytes = new byte[signedLength + MacLength];
            Buffer.BlockCopy(aes.IV, 0, fileBytes, 0, IvLength);
            Buffer.BlockCopy(cipherBytes, 0, fileBytes, IvLength, cipherBytes.Length);
            byte[] mac = ComputeMac(fileBytes, signedLength);
            Buffer.BlockCopy(mac, 0, fileBytes, signedLength, MacLength);
            return fileBytes;
        }
    }

    public bool TryDecrypt(byte[] fileBytes, out int score)
    {
        score = 0;
        if (fileBytes == null || fileBytes.Length < IvLength + AesBlockLength + MacLength)
        {
            return false;
        }

        int signedLength = fileBytes.Length - MacLength;
        byte[] expectedMac = ComputeMac(fileBytes, signedLength);
        if (!MacEquals(expectedMac, fileBytes, signedLength))
        {
            return false;
        }

        byte[] iv = new byte[IvLength];
        Buffer.BlockCopy(fileBytes, 0, iv, 0, IvLength);

        try
        {
            using (Aes aes = CreateAes())
            {
                aes.IV = iv;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] plainBytes = decryptor.TransformFinalBlock(fileBytes, IvLength, signedLength - IvLength);
                    if (plainBytes.Length != sizeof(int))
                    {
                        return false;
                    }
                    score = BitConverter.ToInt32(plainBytes, 0);
                }
            }
        }
        catch (CryptographicException)
        {
            score = 0;
            return false;
        }

        if (score < 0)
        {
            score = 0;
            return false;
        }
        return true;
    }

    private Aes CreateAes()
    {
        Aes aes = Aes.Create();
        aes.Key = encryptionKey;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        return aes;
    }

    private byte[] ComputeMac(byte[] bytes, int length)
    {
        using (HMACSHA256 hmac = new HMACSHA256(macKey))
        {
            return hmac.ComputeHash(bytes, 0, length);
        }
    }

    private static bool MacEquals(byte[] expectedMac, byte[] fileBytes, int macOffset)
    {
        int difference = 0;
        for (int index = 0; index < MacLength; index++)
        {
            difference |= expectedMac[index] ^ fileBytes[macOffset + index];
        }
        return difference == 0;
    }
}
