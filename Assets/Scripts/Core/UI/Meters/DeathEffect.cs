using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathEffect : Singleton<DeathEffect>
{
	[SerializeField] private Image backgroundPanel;
	[SerializeField] private Text distanceRanText;
	[SerializeField] private Button continueButton;
	[SerializeField] private InputField nameInputField;

	private bool dead;
	private bool animationComplete;
	private float timer;
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
		
	}

	public void Death() { dead = true; }

	public void RunAgain() { SceneManager.LoadScene(0); }
}