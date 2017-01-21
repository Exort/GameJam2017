using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HighScoreTool:BaseSingleton<HighScoreTool>,EventListener {
    private DateTime lastHighScoreRefresh;
    private int MinutesBetweenRefresh = 5;
    public List<HighScoreEntry> highScores = new List<HighScoreEntry>();
    public HighScoreTool()
    {

        
    }
    public void StartThread()
    {
        bgw = new System.Threading.Thread(new System.Threading.ThreadStart(BgkThread));
        bgw.Start();
    }
    private bool Quitting = false;
    private void BgkThread()
    {
        while(!Quitting)
        {
            if(lastHighScoreRefresh == null || DateTime.Now.Subtract(lastHighScoreRefresh).TotalMinutes > MinutesBetweenRefresh)
            {
                System.Net.WebClient cli = new System.Net.WebClient();
                string myJSON = cli.DownloadString("http://glacial-earth-23991.herokuapp.com/list");
                HighScoreList hl = JsonUtility.FromJson<HighScoreList>(myJSON);
                highScores = hl.data;
                lastHighScoreRefresh = DateTime.Now;
            }
            System.Threading.Thread.Sleep(100);
           
        }
    }
    System.Threading.Thread bgw;
    public void OnMessage(EventType tp, object param)
    {
        if(tp == EventType.ApplicationExit)
        {
            Quitting = true;
            bgw.Abort();
        }
    }
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
   
    public void SendHighScore(string name, long score)
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
        lastHighScoreRefresh = DateTime.MinValue;


    }
}
