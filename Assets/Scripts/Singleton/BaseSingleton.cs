using UnityEngine;

public abstract class BaseSingleton<T> : MonoBehaviour 
    where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                var gameObject = new GameObject ();
                _instance = gameObject.AddComponent<T>();
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
