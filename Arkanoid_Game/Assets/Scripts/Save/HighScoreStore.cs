using System;
using System.IO;
using UnityEngine;

public class HighScoreStore
{
    private const string Secret = "Arkanoid-Stage1-HighScore-5b1e9c7a";
    private const int ResetHighScore = 0;

    private readonly string filePath;
    private readonly HighScoreCipher cipher;

    public string FilePath => filePath;

    public HighScoreStore(string filePath, HighScoreCipher cipher)
    {
        this.filePath = filePath;
        this.cipher = cipher;
    }

    public static HighScoreStore CreateInPersistentData(string fileName)
    {
        return new HighScoreStore(Path.Combine(Application.persistentDataPath, fileName), new HighScoreCipher(Secret));
    }

    public int Load(out HighScoreLoadStatus status)
    {
        if (!File.Exists(filePath))
        {
            TrySave(ResetHighScore);
            status = HighScoreLoadStatus.CreatedNew;
            return ResetHighScore;
        }

        if (TryReadAllBytes(out byte[] fileBytes) && cipher.TryDecrypt(fileBytes, out int highScore))
        {
            status = HighScoreLoadStatus.Loaded;
            return highScore;
        }

        DeleteFile();
        TrySave(ResetHighScore);
        status = HighScoreLoadStatus.ResetAfterCorruption;
        return ResetHighScore;
    }

    public bool TrySave(int highScore)
    {
        try
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllBytes(filePath, cipher.Encrypt(highScore));
            return true;
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
        {
            return false;
        }
    }

    private bool TryReadAllBytes(out byte[] fileBytes)
    {
        try
        {
            fileBytes = File.ReadAllBytes(filePath);
            return true;
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
        {
            fileBytes = null;
            return false;
        }
    }

    private void DeleteFile()
    {
        try
        {
            File.Delete(filePath);
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
        {
        }
    }
}
