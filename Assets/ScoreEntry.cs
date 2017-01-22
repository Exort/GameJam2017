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
    float previousVerticalAxis;
    float previousTouchAxis;

    VerticalSwipeHelper _verticalSwipeHelper;

    public void PromptName()
    {
        _verticalSwipeHelper = new VerticalSwipeHelper ();
        PlayerName.text = "A  ";
        _nameEnterIndex = 0;
        previousVerticalAxis = 0;
    }
    private bool upPreviouslyKeyDown = false;
    private bool downPreviouslyKeyDown = false;
    void Update()
    {
        if(_nameEnterIndex >= 0)
        {
           
            float verticalAxis = Input.GetAxis("Vertical");
            var touchAxis = _verticalSwipeHelper.UpdateAxisValue ();

            if(verticalAxis > previousVerticalAxis || touchAxis > previousTouchAxis)
            {
                if(previousVerticalAxis < 0 || previousTouchAxis < 0)
                {
                    downPreviouslyKeyDown = false;
                }
                else
                {
                    if(!upPreviouslyKeyDown)
                    {
                        upPreviouslyKeyDown = true;
                        PlayerName.text = ReplaceCharAtIndex(PlayerName.text, CycleChar(PlayerName.text[_nameEnterIndex], 1), _nameEnterIndex);
                    }
                }
            }
            else
            {
                if(verticalAxis < previousVerticalAxis || touchAxis < previousTouchAxis)
                {
                    if(previousVerticalAxis > 0 || previousTouchAxis > 0)
                    {
                        upPreviouslyKeyDown = false;
                    }
                    else
                    {
                        if(!downPreviouslyKeyDown)
                        {
                            downPreviouslyKeyDown = true;
                            PlayerName.text = ReplaceCharAtIndex(PlayerName.text, CycleChar(PlayerName.text[_nameEnterIndex], -1), _nameEnterIndex);
                        }
                    }
                }
            }

            previousVerticalAxis = verticalAxis;
            previousTouchAxis = touchAxis;



            if(Input.anyKeyDown && !upPreviouslyKeyDown && !downPreviouslyKeyDown)
            {
                _nameEnterIndex++;

                if(_nameEnterIndex == NameLength)
                {
                    SubmitName ();
                    return;
                }

                PlayerName.text = ReplaceCharAtIndex(PlayerName.text, 'A', _nameEnterIndex);
            }

            previousVerticalAxis = verticalAxis;
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
        _verticalSwipeHelper = null;
        _nameEnterIndex = -1;
        NameEntered (PlayerName.text, Score.text);
    }
}
