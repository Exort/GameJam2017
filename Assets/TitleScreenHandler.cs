using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenHandler : MonoBehaviour {

    public GameManager theUi;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.anyKeyDown)
        {
            DestroyImmediate(this.gameObject);
        }	
	}
}
