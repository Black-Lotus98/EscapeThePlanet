using UnityEngine;
using System.IO;

// The class is static because there should not be an instance of this class
public static class SaveManager
{
    public static string director = "SaveData";
    public static string fileName = "GameData.dat";

    public static void Save(GameData data)
    {
        if (!DirectoryExists())
            Directory.CreateDirectory(Application.persistentDataPath + "/" + director);

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetFullPath(), AESCrypto.Encrypt(json));
    }

    public static GameData Load()
    {
        if (SaveExists())
        {
            try
            {
                string encrypted = File.ReadAllText(GetFullPath());
                string json = AESCrypto.Decrypt(encrypted);
                return JsonUtility.FromJson<GameData>(json);
            }
            catch
            {
                Debug.LogError("Failed to load save data");
                return null;
            }
        }
        return null;
    }

    public static void ResetGameData(GameData data)
    {
        if (!DirectoryExists())
            Directory.CreateDirectory(Application.persistentDataPath + "/" + director);

        data = new GameData();
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetFullPath(), AESCrypto.Encrypt(json));
    }

    private static bool SaveExists()
    {
        return File.Exists(GetFullPath());
    }

    private static bool DirectoryExists()
    {
        return Directory.Exists(Application.persistentDataPath + "/" + director);
    }

    private static string GetFullPath()
    {
        return Application.persistentDataPath + "/" + director + "/" + fileName;
    }
}
