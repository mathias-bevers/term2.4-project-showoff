using UnityEngine;

public class DEBUG : MonoBehaviour
{
	private const int OFFSET = 10;

	[SerializeField] private Vector2 rectSize;
	[SerializeField] private GUIStyle biggerFont;

	private void Awake()
	{
#if !DEBUG
			Destroy(gameObject);
#endif

		if (FindObjectsOfType<DEBUG>().Length > 1) { Destroy(gameObject); }
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.JoystickButton7)) { PauseEditor(); }

		if (Input.GetKey(KeyCode.JoystickButton6)) { KillPlayer(); }

		if (Input.GetAxis("Debug Vertical") > 0) { KillServer(); }
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10, GetPositionY(1), rectSize.x, rectSize.y), "PAUSE")) { PauseEditor(); }

		if (GUI.Button(new Rect(10, GetPositionY(2), rectSize.x, rectSize.y), "KILL")) { KillPlayer(); }

		if (GUI.Button(new Rect(10, GetPositionY(3), rectSize.x, rectSize.y), "KILL SEVER")) { KillServer(); }

		if (Server.IsInitialized) { GUI.Label(new Rect(Screen.width - rectSize.x * 2, Screen.height - (rectSize.y * 3 + 10), rectSize.x * 2, rectSize.y * 3), Server.Instance.DEBUG_INFO(), biggerFont); }
	}

	private void KillPlayer()
	{
		if (!Player.IsInitialized) { return; }

		Player.Instance.Kill();
	}

	private void PauseEditor() { Debug.Break(); }

	private void KillServer()
	{
		if (!Server.IsInitialized) { return; }

		Destroy(Server.Instance.gameObject);
	}

	private int GetPositionY(int num)
	{
		float result = 0;
		for (int i = 1; i <= num; ++i) { result += rectSize.y + OFFSET; }

		return Screen.height - (int)result;
	}
}