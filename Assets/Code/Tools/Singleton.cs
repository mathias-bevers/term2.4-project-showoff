using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
	private static T instance;

	public static T Instance => GetInstance();

	public static bool IsInitialized => instance != null;

	public virtual void Awake()
	{
		if (instance == null) { instance = this as T; }
		else { Destroy(gameObject); }
	}

	protected virtual void OnDestroy() { instance = null; }

	private static T GetInstance()
	{
		if (instance != null) { return instance; }

		instance = FindObjectOfType<T>();
		if (instance != null) { return instance; }

		GameObject obj = new();
		obj.name = typeof(T).Name;
		instance = obj.AddComponent<T>();
		return instance;
	}
}