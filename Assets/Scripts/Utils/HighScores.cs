using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using UnityEngine;

public class HighScores
{
    private static readonly List<Dictionary<string,string>> Data;

    static HighScores()
    {
        Data = new List<Dictionary<string, string>>();
        for (var i = 0; i < Level.LevelsAmount; i++) 
            Data.Add(new Dictionary<string, string>());
    }

    private static readonly bool[] Fetched = new bool[Level.LevelsAmount];
    public static readonly Action[] WhenFetched = new Action[Level.LevelsAmount];
    public static void SaveFromJson(string json, int l)
    {
        if (string.IsNullOrEmpty(json))
        {
            return;
        }
        var pd = PlayerData.Instance;
        json = json.Trim('{', '}');
        var scores = json.Split(',');
        foreach (var s in scores)
        {
            var t = s.Split(':');
            var id = t[0].Trim('"');
            var score = t[1];
            Data[l][id] = score;
        }
        for (var i = 0; i < Data.Count; i++)
        {
            var pdScore = pd.Scores[i];
            var fScore = Data[i].ContainsKey(pd.ID) ? Data[i][pd.ID] : "0";
            if (pdScore.ToFloat() > fScore.ToFloat())
            {
                Data[i][pd.ID] = pdScore;
                WebUtils.SendScore(i);
            }
            else
            {
                pd.Scores[i] = fScore;
            }
        }
        Fetched[l] = true;
        if (WhenFetched[l] != null)
        {
            WhenFetched[l]();
            WhenFetched[l] = null;
        }
    }

    public static string GetString(int level)
    {
        if (!Fetched[level])
        {
            var pd = PlayerData.Instance;
            Data[level][pd.ID] = pd.Scores[level];
        }
        var result = new List<string>();
        var gotPlayer = false;
        var sincePlayer = 0;
        var n = 1;
        foreach (var entry in Data[level].OrderByDescending(key => float.Parse(key.Value)))
        {
            var s = "<color=grey>" + n + ". </color>";
            if (entry.Key == PlayerData.Instance.ID)
            {
                result.Add(s + "<color=white>" + entry.Value + "</color>\n");
                gotPlayer = true;
            }
            else
            {
                result.Add(s + entry.Value + "\n");
            }
            if (gotPlayer) sincePlayer++;
            if (result.Count > 8) result.RemoveAt(0);
            if (sincePlayer >= 3 && result.Count > 7) break;
            n++;
        }
        var strResult = result.Aggregate("", (current, str) => current + str);
        return strResult;
    }
}