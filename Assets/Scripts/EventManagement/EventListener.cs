public enum EventType
{
    StartGame,
    GameOver,
    ApplicationExit,
    PlayMusic,
    StopMusic
}

public interface EventListener
{
    void OnMessage(EventType t, object param);
}
