using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

// The class is static because there should not be an instance of this class
public static class SaveManager
{
    public static string director = "SaveData";
    public static string fileName = "GameData.dat";

    public static void Save(GameData data)
    {
        if (!DirectoryExists())
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + director);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(GetFullPath());
        bf.Serialize(file, data);
        file.Close();
    }

    public static GameData Load()
    {
        if (SaveExists())
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(GetFullPath(), FileMode.Open);
                GameData data = (GameData)bf.Deserialize(file);
                file.Close();

                return data;
            }
            catch (SerializationException)
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
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + director);
        }
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Player.data";

        // FileStream stream = new FileStream(GetFullPath(), FileMode.Create);
        FileStream file = File.Create(GetFullPath());

        data = new GameData();
        formatter.Serialize(file, data);
        file.Close();
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
