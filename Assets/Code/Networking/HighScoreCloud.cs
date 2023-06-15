using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HighScoreCloud
{
	private const int NAME_PADDING = 25;
	private readonly List<(string, int)> namedScores;

	public HighScoreCloud() { namedScores = new List<(string, int)>(); }

	public void AddScore(string name, int score)
	{
		if (namedScores.Any(namedScore => namedScore.Item1 == name && namedScore.Item2 == score)) { return; }
		namedScores.Add((name, score));
	}

	public void AddScore((string, int) namedScore) => AddScore(namedScore.Item1, namedScore.Item2);

	public List<(string, int)> GetAllScores() => namedScores;

	public override string ToString()
	{
		StringBuilder sb = new($"There are {namedScores.Count} high-score entries in the cloud:\n");

		foreach ((string, int) namedScore in namedScores)
		{
			sb.Append('\t');
			sb.Append((namedScore.Item1 + ':').PadRight(NAME_PADDING));
			sb.Append(namedScore.Item2.ToString());
			sb.Append(Environment.NewLine);
		}

		return sb.ToString();
	}
}