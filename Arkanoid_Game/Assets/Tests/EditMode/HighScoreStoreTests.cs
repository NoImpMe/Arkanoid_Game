using System;
using System.IO;
using System.Text;
using NUnit.Framework;

public class HighScoreStoreTests
{
    private string directory;
    private string filePath;
    private HighScoreCipher cipher;
    private HighScoreStore store;

    [SetUp]
    public void SetUp()
    {
        directory = Path.Combine(Path.GetTempPath(), "ArkanoidHighScoreTests", Guid.NewGuid().ToString("N"));
        filePath = Path.Combine(directory, "highscore.dat");
        cipher = new HighScoreCipher("test-secret");
        store = new HighScoreStore(filePath, cipher);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }

    [Test]
    public void Load_NoFile_ReturnsZeroAndCreatesReadableFile()
    {
        int highScore = store.Load(out HighScoreLoadStatus status);

        Assert.AreEqual(0, highScore);
        Assert.AreEqual(HighScoreLoadStatus.CreatedNew, status);
        Assert.IsTrue(File.Exists(filePath));
        Assert.IsTrue(cipher.TryDecrypt(File.ReadAllBytes(filePath), out int stored));
        Assert.AreEqual(0, stored);
    }

    [Test]
    public void SaveThenLoad_ReturnsSavedHighScore()
    {
        Assert.IsTrue(store.TrySave(4560));

        int highScore = store.Load(out HighScoreLoadStatus status);

        Assert.AreEqual(4560, highScore);
        Assert.AreEqual(HighScoreLoadStatus.Loaded, status);
    }

    [Test]
    public void Load_TamperedFile_ResetsToZeroAndRecreatesValidFile()
    {
        store.TrySave(4560);
        byte[] tampered = File.ReadAllBytes(filePath);
        tampered[18] ^= 0x40;
        File.WriteAllBytes(filePath, tampered);

        int highScore = store.Load(out HighScoreLoadStatus status);

        Assert.AreEqual(0, highScore);
        Assert.AreEqual(HighScoreLoadStatus.ResetAfterCorruption, status);
        CollectionAssert.AreNotEqual(tampered, File.ReadAllBytes(filePath));
        Assert.AreEqual(0, store.Load(out HighScoreLoadStatus reloadStatus));
        Assert.AreEqual(HighScoreLoadStatus.Loaded, reloadStatus);
    }

    [Test]
    public void Load_PlainTextEditedFile_ResetsToZero()
    {
        Directory.CreateDirectory(directory);
        File.WriteAllText(filePath, "99999999", Encoding.UTF8);

        int highScore = store.Load(out HighScoreLoadStatus status);

        Assert.AreEqual(0, highScore);
        Assert.AreEqual(HighScoreLoadStatus.ResetAfterCorruption, status);
        Assert.IsTrue(cipher.TryDecrypt(File.ReadAllBytes(filePath), out int stored));
        Assert.AreEqual(0, stored);
    }

    [Test]
    public void Load_EmptyFile_ResetsToZero()
    {
        Directory.CreateDirectory(directory);
        File.WriteAllBytes(filePath, new byte[0]);

        Assert.AreEqual(0, store.Load(out HighScoreLoadStatus status));
        Assert.AreEqual(HighScoreLoadStatus.ResetAfterCorruption, status);
    }

    [Test]
    public void Load_FileFromOtherSecret_ResetsToZero()
    {
        new HighScoreStore(filePath, new HighScoreCipher("other-secret")).TrySave(9999);

        Assert.AreEqual(0, store.Load(out HighScoreLoadStatus status));
        Assert.AreEqual(HighScoreLoadStatus.ResetAfterCorruption, status);
    }
}
