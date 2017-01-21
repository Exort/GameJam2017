using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour 
{
    public Text ScoreText;
    public Text MultiplierText;
    public Text LevelText;

    public void Reset()
    {
        ScoreText.text = string.Empty;
        MultiplierText.text = string.Empty;
        LevelText.text = string.Empty;
    }
}
