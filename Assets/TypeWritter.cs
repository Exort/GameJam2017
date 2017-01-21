﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TypeWritter : MonoBehaviour {

    Text txt;
    string story;

    void Awake()
    {
        txt = GetComponent<Text>();
        story = txt.text;
        txt.text = "";

        // TODO: add optional delay when to start
        StartCoroutine("PlayText");
    }

    IEnumerator PlayText()
    {
        foreach (char c in story)
        {
            txt.text += c;
            yield return new WaitForSeconds(0.125f);
        }

        while (true)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene("GameScene");
                yield return null;
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
