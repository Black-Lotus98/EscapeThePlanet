using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

// the class is static as 
public static class SaveData
{
    public static void SavePlayerData(CollisionHandler collisionHandler)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Player.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(collisionHandler);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void ResetPlayerData(PlayerData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Player.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        data = new PlayerData();
        formatter.Serialize(stream, data);
        stream.Close();
    }


    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/Player.data";
        // Debug.Log(Application.persistentDataPath + " / Player.data");

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    PlayerData data = formatter.Deserialize(stream) as PlayerData;

                    return data;
                }
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
    // public static void ResetPlayerData(LevelData data)
    // {
    //     BinaryFormatter formatter = new BinaryFormatter();
    //     string path = Application.persistentDataPath + "/LevelData.data";

    //     FileStream stream = new FileStream(path, FileMode.Create);

    //     formatter.Serialize(stream, data);
    //     stream.Close();
    // }
    // public static LevelData LoadLevelData()
    // {
    //     string path = Application.persistentDataPath + "/LevelData.data";
    //     Debug.Log(Application.persistentDataPath + " / LevelData.data");

    //     if (File.Exists(path))
    //     {
    //         BinaryFormatter formatter = new BinaryFormatter();
    //         try
    //         {
    //             using (FileStream stream = new FileStream(path, FileMode.Open))
    //             {
    //                 LevelData data = formatter.Deserialize(stream) as LevelData;

    //                 return data;
    //             }
    //         }
    //         catch (Exception e)
    //         {
    //             Debug.LogError("Error while loading save data: " + e.Message);
    //             return null;
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("Save file not found in " + path);
    //         return null;
    //     }
    // }
}
