using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class HighScorePanel : MonoBehaviour
{
	[SerializeField] private Text numberText;
	[SerializeField] private Text nameText;
	[SerializeField] private Text scoreText;

	public void SetScore(HighScoreData? data)
	{
		if (data == null)
		{
			gameObject.SetActive(false);
			return;
		}

		gameObject.SetActive(true);
		HighScoreData internalData = data.Value;

		if (numberText != null) { numberText.text = internalData.spot.ToString(); }

		if (nameText != null) { nameText.text = internalData.name; }

		if (scoreText != null) { scoreText.text = string.Format("{0:n0}", internalData.score); }
	}

	[Button]
	private void SetDummyScore()
	{
		int randomMeters = Random.Range(1, int.MaxValue);
		HighScoreData data = new HighScoreData(1, "DEBUG", randomMeters);
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