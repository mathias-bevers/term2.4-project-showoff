using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField, Scene] private int sceneToLoad;
	[SerializeField] private Transform infoTextParent;

	private bool loadingScene = false;

	private void Awake() { Application.runInBackground = true; }

	private void Update()
	{
		if(loadingScene) {return;}
		
		if (!Input.GetButtonDown("Submit")) { return; }
		
		RunGame();
	}

	private void RunGame()
	{
		//string ipAddress = ; //TODO: get ip from settings.
		Settings.SERVER_IP = Utils.GetIP4Address();

		loadingScene = true;
		infoTextParent.SetChildrenText("LOADING...");
		
		SceneManager.LoadScene(sceneToLoad);
	}
}