using UnityEngine;
using UnityEngine.UI;

public class HighScorePanel : MonoBehaviour
{
    [SerializeField] Text numberText;
    [SerializeField] Text nameText;
    [SerializeField] Text scoreText;

    public void SetScore(HighScoreData? data)
    {
        if (data == null) gameObject.SetActive(false);
        gameObject.SetActive(true);
        HighScoreData internalData = data.Value;

        if (numberText != null) numberText.text = internalData.spot.ToString();
        if (nameText != null)   nameText.text = internalData.name;
        if (scoreText != null)  scoreText.text = string.Format("{0:n}", internalData.score).Replace(",00", "") + "m";
    }

    private void Update()
    {
        SetScore(new HighScoreData(1, "test!", 1234567890));
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
}
