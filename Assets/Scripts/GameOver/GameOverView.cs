using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameOverView : MonoBehaviour {
    public Transform ScoreContainer;
    public ScoreEntry ScoreEntryObject;

    private bool _done;

    public bool IsDone
    {
        get
        {
            return _done;
        }
    }

    private void SendHighScore(long score)
    {
    }
    public void Fillout(long score)
    {
        while(ScoreContainer.childCount != 0)
        {
            Destroy(ScoreContainer.GetChild(0));
        }

        HighScoreTool.HighScoreEntry newHighScoreEntry = null;
        var highScores = HighScoreTool.Instance.highScores;
        if(highScores.Count == 0 || HighScoreTool.Instance.highScores.Any(x => long.Parse(x.score) < score))
        {
            newHighScoreEntry = new HighScoreTool.HighScoreEntry ();
            newHighScoreEntry.name = string.Empty;
            newHighScoreEntry.score = score.ToString();
            highScores.Add (newHighScoreEntry);
        }

        /*DISPLAY HIGH SCORE SCREEN*/
        foreach (HighScoreTool.HighScoreEntry entr in HighScoreTool.Instance.highScores)
        {
            ScoreEntry scoreEntry = Instantiate(ScoreEntryObject, ScoreContainer, false);
            scoreEntry.Score.text = entr.score;
            scoreEntry.PlayerName.text = entr.name;
            if(entr == newHighScoreEntry)
            {
                scoreEntry.NameEntered += OnNameEntered;
                scoreEntry.PromptName ();
            }
        }
    }

    void OnNameEntered(string playerName, string score)
    {
        HighScoreTool.Instance.SendHighScore(playerName, long.Parse(score));
        _done = true;
    }

	// Update is called once per frame
	void Update () 
    {
	}
}
