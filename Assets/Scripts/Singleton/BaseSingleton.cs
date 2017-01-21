using UnityEngine;

public abstract class BaseSingleton<T> : MonoBehaviour 
    where T : class, new()
{
    protected T _instance;

    public T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new T ();
            }
            return _instance;
        }
    }

    protected void OnDestroy()
    {
        if(this == _instance)
        {
            _instance = null;
        }
    }
}
