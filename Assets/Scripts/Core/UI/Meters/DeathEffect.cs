using System;
using UnityEngine;
using UnityEngine.UI;

public class DeathEffect : Singleton<DeathEffect>
{
	[SerializeField] private GameObject dataBaseCanvas;
	[SerializeField] private Image backgroundPanel;
	[SerializeField] private Transform distanceRanParent;
	[SerializeField] private Button continueButton;
	[SerializeField] private NameInput nameInput;


	private bool animationComplete;
	private bool sentScoreToServer;
	private bool dead;
	private float distanceRan;
	private float timer = -2;

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
		distanceRanParent.SetChildrenText($"YOU RAN {distanceRan:n0} METERS");
		PowerupDisplayHandler.Instance.gameObject.SetActive(false);

		animationComplete = true;
	}

	private void OnContinueClicked()
	{
		string trimmedName = nameInput.GetName();
		trimmedName = trimmedName.Trim();

		try
		{
			HighScoreManager.Instance.SendHighScoreToServer(trimmedName, (int)distanceRan);
			dataBaseCanvas.SetActive(true);
			gameObject.SetActive(false);
			continueButton.onClick.RemoveAllListeners();
		}
		catch (ArgumentException e) { distanceRanParent.SetChildrenText(e.Message.ColorRichText(Color.red)); }
	}

	public void Death() { dead = true; }
}