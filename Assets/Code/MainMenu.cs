using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField, Scene] private int sceneToLoad;
	[SerializeField] private Button startButton;
	[SerializeField] private Button quitButton;

	private void Awake()
	{
		Application.runInBackground = true;
		
		startButton.onClick.AddListener(() => SceneManager.LoadScene(sceneToLoad));
		quitButton.onClick.AddListener(Application.Quit);
	}
}