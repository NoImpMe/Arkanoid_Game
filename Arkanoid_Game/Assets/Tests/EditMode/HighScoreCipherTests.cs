using NUnit.Framework;

public class HighScoreCipherTests
{
    private const string Secret = "test-secret";

    [TestCase(0)]
    [TestCase(1230)]
    [TestCase(int.MaxValue)]
    public void EncryptThenDecrypt_ReturnsSameScore(int score)
    {
        HighScoreCipher cipher = new HighScoreCipher(Secret);

        Assert.IsTrue(cipher.TryDecrypt(cipher.Encrypt(score), out int decrypted));
        Assert.AreEqual(score, decrypted);
    }

    [Test]
    public void Encrypt_SameScoreTwice_ProducesDifferentBytes()
    {
        HighScoreCipher cipher = new HighScoreCipher(Secret);

        CollectionAssert.AreNotEqual(cipher.Encrypt(500), cipher.Encrypt(500));
    }

    [Test]
    public void Encrypt_DoesNotContainPlainScoreBytes()
    {
        HighScoreCipher cipher = new HighScoreCipher(Secret);
        byte[] plainBytes = System.BitConverter.GetBytes(123456789);
        byte[] fileBytes = cipher.Encrypt(123456789);

        bool containsPlain = false;
        for (int start = 0; start + plainBytes.Length <= fileBytes.Length; start++)
        {
            bool match = true;
            for (int offset = 0; offset < plainBytes.Length; offset++)
            {
                match &= fileBytes[start + offset] == plainBytes[offset];
            }
            containsPlain |= match;
        }

        Assert.IsFalse(containsPlain);
    }

    [Test]
    public void TryDecrypt_CipherByteChanged_Fails()
    {
        HighScoreCipher cipher = new HighScoreCipher(Secret);
        byte[] fileBytes = cipher.Encrypt(1230);
        fileBytes[20] ^= 0x01;

        Assert.IsFalse(cipher.TryDecrypt(fileBytes, out int score));
        Assert.AreEqual(0, score);
    }

    [Test]
    public void TryDecrypt_MacByteChanged_Fails()
    {
        HighScoreCipher cipher = new HighScoreCipher(Secret);
        byte[] fileBytes = cipher.Encrypt(1230);
        fileBytes[fileBytes.Length - 1] ^= 0x01;

        Assert.IsFalse(cipher.TryDecrypt(fileBytes, out _));
    }

    [Test]
    public void TryDecrypt_IvByteChanged_Fails()
    {
        HighScoreCipher cipher = new HighScoreCipher(Secret);
        byte[] fileBytes = cipher.Encrypt(1230);
        fileBytes[0] ^= 0x01;

        Assert.IsFalse(cipher.TryDecrypt(fileBytes, out _));
    }

    [Test]
    public void TryDecrypt_EmptyNullOrTruncated_Fails()
    {
        HighScoreCipher cipher = new HighScoreCipher(Secret);
        byte[] fileBytes = cipher.Encrypt(1230);
        byte[] truncated = new byte[fileBytes.Length - 1];
        System.Array.Copy(fileBytes, truncated, truncated.Length);

        Assert.IsFalse(cipher.TryDecrypt(null, out _));
        Assert.IsFalse(cipher.TryDecrypt(new byte[0], out _));
        Assert.IsFalse(cipher.TryDecrypt(truncated, out _));
    }

    [Test]
    public void TryDecrypt_WithDifferentSecret_Fails()
    {
        byte[] fileBytes = new HighScoreCipher(Secret).Encrypt(1230);

        Assert.IsFalse(new HighScoreCipher("other-secret").TryDecrypt(fileBytes, out _));
    }

    [Test]
    public void TryDecrypt_NegativeScore_Fails()
    {
        HighScoreCipher cipher = new HighScoreCipher(Secret);

        Assert.IsFalse(cipher.TryDecrypt(cipher.Encrypt(-5), out int score));
        Assert.AreEqual(0, score);
    }
}
