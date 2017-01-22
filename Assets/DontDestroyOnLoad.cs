using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour, EventListener {

    private AudioSource audioSource;

    public void OnMessage(EventType eventType, object param)
    {
        switch(eventType)
        {
            case EventType.PlayMusic:
                audioSource.Play();
                break;
            case EventType.StopMusic:
                audioSource.Stop();
                break;
        }
    }

    void Start ()
    {
        audioSource = GetComponent<AudioSource>();

        EventManager.Instance.Register(this);
    }
}
