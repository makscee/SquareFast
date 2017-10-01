using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebUtils
{
    [Serializable]
    public class WebData
    {
        public string Id;
        public string Score;

        public WebData(string id, string score)
        {
            Id = id;
            Score = score;
        }
    }
    
    public static void SendScore(int level)
    {
        CameraScript.Instance.StartCoroutine(UploadScore(level));
    }
    
    private static IEnumerator UploadScore(int level)
    {
        var pd = PlayerData.Instance;
        var data = string.Format("{{ \"{0}\":{1} }}", pd.ID, pd.Scores[level]);
        Debug.Log(data);
        var www = UnityWebRequest.Put("https://squarefast-5127e.firebaseio.com/scores/" + level + ".json", data);
        www.method = "PATCH";
        yield return www.Send();

        Debug.Log(www.isNetworkError ? www.error : "Upload complete!");
    }
    
    public static void FetchScores() {
        CameraScript.Instance.StartCoroutine(DownloadScores());
    }
 
    private static IEnumerator DownloadScores() {
        var www = UnityWebRequest.Get("https://squarefast-5127e.firebaseio.com/scores.json");
        yield return www.Send();
 
        if(www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log(www.downloadHandler.text);
            HighScores.SaveFromJson(www.downloadHandler.text);
        }
    }
}