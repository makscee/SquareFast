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
    public string[] Scores = new string[Level.LevelsAmount];

    public PlayerData()
    {
        for (var i = 0; i < Level.LevelsAmount; i++)
        {
            Scores[i] = "0";
        }
    }
    public override string ToString()
    {
        var scores = Scores.Aggregate("", (current, score) => current + (" " + score));
        return "ID: " + ID + " " + scores;
    }
}

public class Saves
{
    public static void Save()
    {
        SaveToFile();
        SaveToPrefs();
        Debug.Log("Saved " + PlayerData.Instance);
    }

    private static void SaveToFile()
    {
        var bf = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + "/playerData.gd");
        bf.Serialize(file, PlayerData.Instance);
        file.Close();
    }

    private static void SaveToPrefs()
    {
        var pd = PlayerData.Instance;
        PlayerPrefs.SetString("id", pd.ID);
        for (var i = 0; i < pd.Scores.Length; i++)
        {
            var score = PlayerPrefs.GetString("score" + i, "0").ToFloat();
            if (score < pd.Scores[i].ToFloat())
            {
                PlayerPrefs.SetString("score" + i, pd.Scores[i]);
            }
        }
    }

    public static void Load()
    {
        TryLoadFromFile();
        TryLoadFromPrefs();
        if (PlayerData.Instance == null)
        {
            var pd = new PlayerData();
            pd.ID = SystemInfo.deviceUniqueIdentifier;
            if (pd.ID == SystemInfo.unsupportedIdentifier)
                pd.ID = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
            PlayerData.Instance = pd;
            Debug.Log("Created " + PlayerData.Instance);
            Save();
        }
    }

    private static void TryLoadFromPrefs()
    {
        if (!PlayerPrefs.HasKey("id"))
        {
            Debug.Log("Couldn't load from prefs.");
            return;
        }
        var id = PlayerPrefs.GetString("id");
        Debug.Log("Prefs id: " + id);
        if (PlayerData.Instance == null)
        {
            PlayerData.Instance = new PlayerData();
        }
        var pd = PlayerData.Instance;
        pd.ID = id;
        for (var i = 0; i < Level.LevelsAmount; i++)
        {
            var scoreStr = PlayerPrefs.GetString("score" + i, "0");
            var score = scoreStr.ToFloat();
            if (score > pd.Scores[i].ToFloat())
            {
                pd.Scores[i] = scoreStr;
            }
        }
        Debug.Log("Loaded from prefs " + pd);
    }

    private static void TryLoadFromFile()
    {
        if (!File.Exists(Application.persistentDataPath + "/playerData.gd"))
        {
            return;
        }
        var bf = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/playerData.gd", FileMode.Open);
        try
        {
            PlayerData.Instance = (PlayerData) bf.Deserialize(file);
        }
        catch (SerializationException e)
        {
            file.Close();
            File.Delete(Application.persistentDataPath + "/playerData.gd");
            return;
        }
        file.Close();
        Debug.Log("Loaded from file " + PlayerData.Instance);
    }
}