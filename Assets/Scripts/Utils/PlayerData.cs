using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public static PlayerData Instance;
    public string ID;
    public string[] Scores = new string[6];
    public override string ToString()
    {
        var scores = Scores.Aggregate("", (current, score) => current + (" " + score));
        return "ID: " + ID + " " + scores;
    }
}

public class Saves
{
    public static void Save() {
        var bf = new BinaryFormatter();
        var file = File.Create (Application.persistentDataPath + "/playerData.gd");
        bf.Serialize(file, PlayerData.Instance);
        Debug.Log("Saved " + PlayerData.Instance);
        file.Close();
    }
    
    public static void Load() {
        if (!File.Exists(Application.persistentDataPath + "/playerData.gd"))
        {
            var pd = new PlayerData();
            pd.ID = SystemInfo.deviceUniqueIdentifier;
            if (pd.ID == SystemInfo.unsupportedIdentifier) pd.ID = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
            PlayerData.Instance = pd;
            Debug.Log("Created " + PlayerData.Instance);
            Save();
            return;
        }
        var bf = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/playerData.gd", FileMode.Open);
        try
        {
            PlayerData.Instance = (PlayerData)bf.Deserialize(file);
        }
        catch (SerializationException e)
        {
            file.Close();
            File.Delete(Application.persistentDataPath + "/playerData.gd");
            Load();
            return;
        }
        file.Close();
        Debug.Log("Loaded " + PlayerData.Instance);
    }
}