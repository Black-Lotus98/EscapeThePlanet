using UnityEngine;
using System.IO;
using System;

// The class is static because there should not be an instance of this class
public static class SaveData
{
    public static void SavePlayerData(CollisionHandler collisionHandler)
    {
        string path = Application.persistentDataPath + "/Player.data";
        PlayerData data = new PlayerData(collisionHandler);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, AESCrypto.Encrypt(json));
    }

    public static void ResetPlayerData(PlayerData data)
    {
        string path = Application.persistentDataPath + "/Player.data";
        data = new PlayerData();
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, AESCrypto.Encrypt(json));
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/Player.data";

        if (File.Exists(path))
        {
            try
            {
                string encrypted = File.ReadAllText(path);
                string json = AESCrypto.Decrypt(encrypted);
                return JsonUtility.FromJson<PlayerData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while loading save data: " + e.Message);
                return null;
            }
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
