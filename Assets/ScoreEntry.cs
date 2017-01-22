using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScoreEntry : MonoBehaviour 
{
    const int NameLength = 3;

    public Text PlayerName;
    public Text Score;

    public event Action<string,string> NameEntered = delegate{};

    int _nameEnterIndex = -1;
    float _previousVerticalAxis;

    public void PromptName()
    {
        PlayerName.text = "A  ";
        _nameEnterIndex = 0;
        _previousVerticalAxis = 0;
    }

    void Update()
    {
        if(_nameEnterIndex >= 0)
        {
            float verticalAxis = Input.GetAxis("Vertical");

            // Up
            if (Mathf.Approximately(_previousVerticalAxis, 0f) && verticalAxis > 0)
            {
                PlayerName.text = ReplaceCharAtIndex(PlayerName.text, CycleChar(PlayerName.text [_nameEnterIndex], 1), _nameEnterIndex);
            }
            // Down
            else if (Mathf.Approximately(_previousVerticalAxis, 0f) &&  verticalAxis < 0)
            {
                PlayerName.text = ReplaceCharAtIndex(PlayerName.text, CycleChar(PlayerName.text [_nameEnterIndex], -1), _nameEnterIndex);
            }
            else if(Input.GetButtonDown("Jump"))
            {
                _nameEnterIndex++;

                if(_nameEnterIndex == NameLength)
                {
                    SubmitName ();
                    return;
                }

                PlayerName.text = ReplaceCharAtIndex(PlayerName.text, 'A', _nameEnterIndex);
            }

            _previousVerticalAxis = verticalAxis;
        }
    }

    private char CycleChar(char character, int delta)
    {
        return (char)(character + delta);
    }

    private string ReplaceCharAtIndex(string str, char newChar, int index )
    {
        var chars = str.ToCharArray ();
        chars [index] = newChar;
        return new string (chars);
    }

    public void SubmitName()
    {
        _nameEnterIndex = -1;
        NameEntered (PlayerName.text, Score.text);
    }
}
