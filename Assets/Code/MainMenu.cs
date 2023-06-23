using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField, Scene] private int sceneToLoad;

	private void Awake() { Application.runInBackground = true; }

	private void Update()
	{
		if (!Input.GetButtonDown("Submit")) { return; }

		Debug.Log("SUBMIT PRESSED");
		RunGame();
	}

	private void RunGame()
	{
		//string ipAddress = ; //TODO: get ip from settings.
		Settings.SERVER_IP = Utils.GetIP4Address();

		SceneManager.LoadScene(sceneToLoad);
	}
}