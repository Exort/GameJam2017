using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseSingleton<GameManager> 
{
    public List<GameObject> Lanes;
    public PlayerCharacter PlayerPrefab;
    public float PlayerOffset = -2.25f;

    public PlayerCharacter PlayerCharacter { get; private set; }

    public int CurrentLane { get; private set; }

    private void StartGame()
    {
        CleanUp ();
        PlayerCharacter = Instantiate (PlayerPrefab);
        PlayerCharacter.transform.localPosition = new Vector3(PlayerOffset, 0, 0);
        ChangeLane (2);
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
            Destroy (PlayerCharacter);
            PlayerCharacter = null;
        }
    }

    private void Update()
    {
        //todo use input manager
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartGame ();
        }

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeLane (Mathf.Max (0, CurrentLane - 1));
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeLane (Mathf.Min (Lanes.Count - 1, CurrentLane + 1));
        }
    }
}
