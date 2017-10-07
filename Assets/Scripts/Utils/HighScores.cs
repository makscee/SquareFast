using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

public class HighScores
{
    public static List<Dictionary<string,string>> Data;

    public static bool fetched = false;
    public static void SaveFromJson(string json)
    {
        var levelNum = 0;
        Data = new List<Dictionary<string, string>>();
        var pd = PlayerData.Instance;
        while (json.IndexOf('{') != -1)
        {
            var startLevel = json.IndexOf('{');
            var endLevel = json.IndexOf('}');
            var level = json.Substring(startLevel + 1, endLevel - startLevel - 1);
            Data.Add(new Dictionary<string, string>());
            var scores = level.Split(',');
            foreach (var s in scores)
            {
                var t = s.Split(':');
                var id = t[0].Trim('"');
                var score = t[1];
                Data[levelNum][id] = score;
            }
            levelNum++;
            json = json.Remove(0, endLevel + 1);
        }
        if (pd != null)
        {
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
        }
        fetched = true;
    }

    public static string GetString(int level)
    {
        if (!fetched)
        {
            Data = new List<Dictionary<string, string>>();
            var i = 0;
            var pd = PlayerData.Instance;
            foreach (var score in pd.Scores)
            {
                Data.Add(new Dictionary<string, string>());
                Data[i][pd.ID] = score;
                i++;
            }
        }
        var result = new List<string>();
        var gotPlayer = false;
        var sincePlayer = 0;
        foreach (var entry in Data[level].OrderByDescending(key => float.Parse(key.Value)))
        {
            if (entry.Key == PlayerData.Instance.ID)
            {
                result.Add("<color=white>" + entry.Value + "</color>\n");
                gotPlayer = true;
            }
            else
            {
                result.Add(entry.Value + "\n");
            }
            if (gotPlayer) sincePlayer++;
            if (result.Count > 8) result.RemoveAt(0);
            if (sincePlayer >= 3 && result.Count > 7) break;
        }
        var strResult = result.Aggregate("", (current, str) => current + str);
        return strResult;
    }
}