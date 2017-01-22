using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScoreEntry : MonoBehaviour 
{
    public Text PlayerName;
    public Text Score;

    public event Action<string,string> NameEntered = delegate{};

    int _nameEnterIndex = -1;

    public void PromptName()
    {
        _nameEnterIndex = 0;
    }

    void Update()
    {
        if(_nameEnterIndex >= 0)
        {
            PlayerName.text = "temp";
            SubmitName ();
        }
    }

    public void SubmitName()
    {
        _nameEnterIndex = -1;
        NameEntered (PlayerName.text, Score.text);
    }
}
