using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class HighScorePanel : MonoBehaviour
{
	[SerializeField] private Transform spotParent;
	[SerializeField] private Transform nameParent;
	[SerializeField] private Transform scoreParent;

	private void Awake()
	{
		if (ReferenceEquals(spotParent, null)) { throw new UnassignedReferenceException($"{nameof(spotParent)} is not set in the editor!"); }

		if (ReferenceEquals(nameParent, null)) { throw new UnassignedReferenceException($"{nameof(nameParent)} is not set in the editor!"); }

		if (ReferenceEquals(scoreParent, null)) { throw new UnassignedReferenceException($"{nameof(scoreParent)} is not set in the editor!"); }
	}

	public void SetScore(HighScoreData? data)
	{
		if (data == null)
		{
			gameObject.SetActive(false);
			return;
		}

		gameObject.SetActive(true);
		HighScoreData internalData = data.Value;

		SetChildrenText(spotParent, $"{internalData.spot}.");
		SetChildrenText(nameParent, internalData.name);
		SetChildrenText(scoreParent, $"{internalData.score:n0}");
	}

	private static void SetChildrenText(Transform parent, string value)
	{
		foreach (Transform child in parent)
		{
			Text childText = child.GetComponentThrow<Text>();
			childText.text = value;
		}
	}


	[Button]
	private void SetDummyScore()
	{
		HighScoreData data = new(5, "MMM", int.MaxValue);
		SetScore(data);
	}
}

public struct HighScoreData
{
	public int spot;
	public string name;
	public int score;

	public HighScoreData(int spot, string name, int score)
	{
		this.spot = spot;
		this.name = name;
		this.score = score;
	}

	public string FormatDistance()
	{
		string distance = $"{score:N0}m";

		return distance;
	}
}