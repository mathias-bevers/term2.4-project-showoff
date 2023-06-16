using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saxion_provided;
using UnityEngine;

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

	public void ProcessPacket(ServerClient sender, HighScoreServerObject serverObject)
	{
		bool closeClient = false;
		switch (serverObject)
		{
			case AddHighScores addingScoreList: 
				foreach ((string, int) score in addingScoreList.scores) { AddScore(score.Item1, score.Item2); }
				break;

			case AddHighScore scoreToAdd:
				AddScore(scoreToAdd.name, scoreToAdd.score);
				// Debug.Log($"adding score {scoreToAdd}\n{this}");
				closeClient = true;
				break;

			default: throw new ArgumentException( $"SERVER: can not process type {serverObject.GetType().Name}", nameof(serverObject));
		}
		
		Packet bulkPacket = new();
		GetHighScores getHighScores = new GetHighScores(namedScores, closeClient);
		bulkPacket.Write(getHighScores);
		Debug.Log($"Sending highscore to client {sender.id}:\n{getHighScores}");
		parentServer.WriteToClient(sender, bulkPacket);
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