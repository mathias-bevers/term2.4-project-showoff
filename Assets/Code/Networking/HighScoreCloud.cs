using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saxion_provided;

public class HighScoreCloud
{
	private const int NAME_PADDING = 25;
	private readonly List<(string, int)> namedScores;
	private Server parentServer;

	public HighScoreCloud(Server parentServer)
	{
		this.parentServer = parentServer;
		namedScores = new List<(string, int)>();
	}

	private void AddScore(string name, int score)
	{
		if (namedScores.Any(namedScore => namedScore.Item1 == name && namedScore.Item2 == score)) { return; }
		namedScores.Add((name, score));
	}

	private void AddScore((string, int) namedScore) => AddScore(namedScore.Item1, namedScore.Item2);

	public IEnumerable<(string, int)> GetAllScores() => namedScores;

	public void ProcessPacket(ServerClient sender, HighScoreServerObject serverObject)
	{
		switch (serverObject)
		{
			case AddHighScores list: 
				foreach ((string, int) score in list.scores) { AddScore(score); }
				Packet listPacket = new();
				listPacket.Write(new AddHighScores(GetAllScores()));
				parentServer.WriteToClient(sender, listPacket, this);
				break;

			case AddHighScore score:
				break;
			
			default: throw new ArgumentException( $"can not process type {serverObject.GetType().Name}", nameof(serverObject));
		}
	}

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