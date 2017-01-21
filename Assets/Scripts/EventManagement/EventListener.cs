using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    public enum EventType
    {
        StartGame,
        GameOver
    }
    public interface EventListener
    {
        void OnMessage(EventType t, object param);
    }
