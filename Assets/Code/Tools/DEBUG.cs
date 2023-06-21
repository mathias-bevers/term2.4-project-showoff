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
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.JoystickButton7))
		{
			Debug.Break();
			return;
		}

		if (!Input.GetKey(KeyCode.JoystickButton6)) { return; }

		if (!Player.IsInitialized) { return; }

		Player.Instance.Kill();
	}

	private void OnGUI()
	{
		PauseButton();
		KillButton();

		if (Server.IsInitialized) { GUI.Label(new Rect(10, GetPositionY(3), rectSize.x * 2, rectSize.y * 3), Server.Instance.DEBUG_INFO(), biggerFont); }
	}

	private int GetPositionY(int num)
	{
		float result = 0;
		for (int i = 1; i <= num; ++i) { result += (rectSize.y + OFFSET) * i; }

		return Screen.height - (int)result;
	}

	private void PauseButton()
	{
		if (!GUI.Button(new Rect(10, GetPositionY(1), rectSize.x, rectSize.y), "PAUSE")) { return; }

		Debug.Break();
	}

	private void KillButton()
	{
		if (!GUI.Button(new Rect(10, GetPositionY(2), rectSize.x, rectSize.y), "KILL")) { return; }

		if (!Player.IsInitialized) { return; }

		Player.Instance.Kill();
	}
}