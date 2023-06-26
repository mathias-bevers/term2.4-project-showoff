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

		spotParent.SetChildrenText($"{internalData.spot}.");
		nameParent.SetChildrenText( internalData.name);
		scoreParent.SetChildrenText( $"{internalData.score:n0}");
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