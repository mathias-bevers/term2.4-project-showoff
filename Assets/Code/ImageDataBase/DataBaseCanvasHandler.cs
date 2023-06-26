using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DataBaseCanvasHandler : MonoBehaviour
{
	[SerializeField] private GameObject[] toEnable;
	[SerializeField] private GameObject[] toDisable;
	[SerializeField] private Button continueButton;

	private void OnEnable()
	{
		continueButton.onClick.AddListener(CloseExplanation);
		EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
	}


	private void CloseExplanation()
	{
		foreach (GameObject go in toDisable) { go.SetActive(false); }

		foreach (GameObject go in toEnable) { go.SetActive(true); }
	}
}