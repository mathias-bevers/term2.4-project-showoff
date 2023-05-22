using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    static T instance;

    public static T Instance
    {
        get => GetInstance();
    }

    static T GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<T>();
        if (instance != null) return instance;

        GameObject obj = new GameObject();
        obj.name = typeof(T).Name;
        instance = obj.AddComponent<T>();
        return instance;
    }

    public virtual void Awake()
    {
        if (instance == null)
            instance = this as T;
        else { Debug.Log("Destroy From: " + GetType().Name); Destroy(gameObject); }
    }
}