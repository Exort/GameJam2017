using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HighScoreTool {

    [System.Serializable]
    public class HighScoreEntry
    {
        public string name;
        public string score;
    }
    [System.Serializable]
    public class ProperHighScoreEntry
    {
        public string name;
        public long score;
    }
    [System.Serializable]
    public class HighScoreList
    {
        public List<HighScoreEntry> data;
    }
    public static List<HighScoreEntry> FetchHighScores()
    {
        System.Net.WebClient cli = new System.Net.WebClient();
        string myJSON = cli.DownloadString("http://glacial-earth-23991.herokuapp.com/list");
        HighScoreList hl = JsonUtility.FromJson<HighScoreList>(myJSON);

        return hl.data;
    }
    public static List<HighScoreEntry> SendHighScore(string name, long score)
    {
        ProperHighScoreEntry phs = new ProperHighScoreEntry();
        phs.name = name;
        phs.score = score;
        System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("http://glacial-earth-23991.herokuapp.com/add");
        req.Method = "POST";
        System.IO.StreamWriter wri = new System.IO.StreamWriter(req.GetRequestStream());
        wri.Write(JsonUtility.ToJson(phs));
        wri.Close();
        System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse();
        System.IO.StreamReader rdr = new System.IO.StreamReader(resp.GetResponseStream());
        string answer = rdr.ReadToEnd();
        Debug.Log(answer);
        rdr.Close();
        return FetchHighScores();
    }
}
