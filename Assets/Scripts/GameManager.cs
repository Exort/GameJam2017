using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager> 
{

    public GameOverView GameOverScreenPrefab;
    public List<GameObject> Lanes;
    public PlayerCharacter PlayerPrefab;
    public float PlayerOffset = -2.25f;
    public string PlayerName { get; set; }
    public long Score = 0;
    public PlayerCharacter PlayerCharacter { get; private set; }

    public int CurrentLane { get; private set; }

    void Start()
    {
        CleanUp ();
    }

    private void StartGame()
    {
        CleanUp ();
        Score = 0;
        PlayerName = "MTL";
        PlayerCharacter = Instantiate (PlayerPrefab);
        PlayerCharacter.transform.localPosition = new Vector3(PlayerOffset, 0, 0);
        ChangeLane (2);
    }
   
    private void GameOver()
    {
        //Remove
        Score = 5;
   

        GameOverView gv = Instantiate(GameOverScreenPrefab);
        gv.Fillout(Score);
      
    }
    private void ChangeLane(int laneIndex)
    {
        if(laneIndex != CurrentLane)
        {
            var localPosition = PlayerCharacter.transform.localPosition;
            PlayerCharacter.transform.SetParent (Lanes [laneIndex].transform, false);
            PlayerCharacter.transform.localPosition = localPosition;

            CurrentLane = laneIndex;
        }
    }

    private void CleanUp()
    {
        if(PlayerCharacter != null)
        {
            DestroyImmediate (PlayerCharacter.gameObject);
            PlayerCharacter = null;
        }
        CurrentLane = -1;
    }

    private void Update()
    {
        //todo use input manager
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartGame ();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeLane (Mathf.Max (0, CurrentLane - 1));
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeLane (Mathf.Min (Lanes.Count - 1, CurrentLane + 1));
        }
    }
}
