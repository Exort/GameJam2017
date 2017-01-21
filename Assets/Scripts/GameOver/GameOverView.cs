using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour {
    public Transform ScoreContainer;
    public GameObject ScoreEntryObject;
    public EnterYourName EnterYourNameTemplate;
    private List<GameObject> scoreList = new List<GameObject>();
    public List<HighScoreTool.HighScoreEntry> currentHighScores;
    // Use this for initialization
    void Start () {
        currentHighScores = new List<HighScoreTool.HighScoreEntry>();

    }
    private void SendHighScore(long score)
    {
        HighScoreTool.Instance.SendHighScore("MTL", score);
       // currentHighScores = HighScoreTool.SendHighScore("MTL", score);
    }
    public void Fillout(long score)
    {

      
        while(ScoreContainer.childCount != 0)
        {
            Destroy(ScoreContainer.GetChild(0));
        }

        /*DISPLAY HIGH SCORE SCREEN*/
        foreach (HighScoreTool.HighScoreEntry entr in HighScoreTool.Instance.highScores)
        {
            GameObject obj = Instantiate(ScoreEntryObject, ScoreContainer);
            obj.GetComponent<Text>().text = entr.name + " - " + entr.score;
            scoreList.Add(obj);
        }
        if (score > long.Parse(currentHighScores[currentHighScores.Count - 1].score))
        {

            Debug.Log("NEW HIGH SCORE!");
            /*Should pop Enter Name UI*/
            EnterYourName entr = Instantiate(EnterYourNameTemplate, ScoreContainer);
            SendHighScore(score);


        }
    }
	// Update is called once per frame
	void Update () {
		if(Input.anyKeyDown)
        {

            DestroyImmediate(this.gameObject);
            Debug.Log("Destroy high score");
        }
	}
}
