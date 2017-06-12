using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private T instance;
    public T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        instance = (T)this;
    }
}