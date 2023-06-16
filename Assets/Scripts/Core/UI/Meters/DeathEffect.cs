using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathEffect : Singleton<DeathEffect>
{
	[SerializeField, Scene] private int mainMenuScene;
	[SerializeField] private Image backgroundPanel;
	[SerializeField] private Text distanceRanText;
	[SerializeField] private Button continueButton;
	[SerializeField] private InputField nameInputField;

	private bool animationComplete;
	private bool sentScoreToServer;
	private float distanceRan;
	bool dead = false;
    float timer = -2;
    
	private Transform[] childTransforms;

	public override void Awake()
	{
		base.Awake();
		
		continueButton.onClick.AddListener(OnContinueClicked);

		backgroundPanel.gameObject.SetActive(false);
		childTransforms = backgroundPanel.transform.GetAllChildren();

		foreach (Transform childTransform in childTransforms) { childTransform.gameObject.SetActive(false); }
	}

	private void Update()
	{
		if (!dead) { return; }

		if (animationComplete) { return; }

		backgroundPanel.gameObject.SetActive(true);
		timer += Time.deltaTime;

		float filler = Mathf.Clamp01(Utils.Map(timer, 0, 2, 0, 1));

		backgroundPanel.fillAmount = filler;

		if (timer < 2) { return; }

		AnimationCompleted();
	}

	private void AnimationCompleted()
	{
		foreach (Transform childObject in childTransforms) { childObject.gameObject.SetActive(true); }

		distanceRan = FindObjectOfType<MapWalker>().TotalMetersRan;
		distanceRanText.text = $"You ran {distanceRan:n0} meters";

		animationComplete = true;
	}

	private void OnContinueClicked()
	{
		if (sentScoreToServer)
		{
			SceneManager.LoadScene(mainMenuScene);
		}
		
		string trimmedName = nameInputField.text.Trim();
		
		try
		{
			HighScoreManager.Instance.SendHighScoreToServer(trimmedName, (int)distanceRan);
			continueButton.GetComponentInChildren<Text>().text = "Back to menu";
			sentScoreToServer = true;
		}
		catch (ArgumentException e)
		{
			distanceRanText.text = e.Message.ColorRichText(Color.red);
		}
	}

	public void Death() { dead = true; }
}