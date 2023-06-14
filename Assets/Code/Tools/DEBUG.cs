using UnityEngine;

public class DEBUG : MonoBehaviour
{
	private const int OFFSET = 10;
	[SerializeField] private Vector2 rectSize;

	private void OnGUI()
	{
		PauseButton();
		KillButton();
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

		Player.Instance.Kill();
	}
}