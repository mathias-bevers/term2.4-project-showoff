using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using saxion_provided;

public class GetHighScores : HighScoreServerObject
{
	public List<(string, int)> scores { get; }
	public bool closeClient { get; private set; }

	public GetHighScores()
	{
		scores = new List<(string, int)>();
		closeClient = false;
	}

	public GetHighScores(IEnumerable<(string, int)> scores, bool closeClient)
	{
		this.scores = scores.ToList();
		this.closeClient = closeClient;
	}

	public override void Serialize(Packet packet)
	{
		packet.Write(scores.Count);

		for (int i = 0; i < scores.Count; ++i)
		{
			packet.Write(scores[i].Item1);
			packet.Write(scores[i].Item2);
		}
		
		packet.Write(closeClient);
	}

	public override void Deserialize(Packet packet)
	{
		int count = packet.ReadInt();

		for (int i = 0; i < count; ++i)
		{
			string name = packet.ReadString();
			int score = packet.ReadInt();
				
			scores.Add((name, score));
		}

		closeClient = packet.ReadBool();
	}

	public override string ToString()
	{
		StringBuilder sb = new("Scores in object: ");

		foreach ((string, int) namedScore in scores)
		{
			sb.Append('\t');
			sb.Append(namedScore.Item1);
			sb.Append(": ");
			sb.Append(namedScore.Item2);
			sb.Append(Environment.NewLine);
		}

		return sb.ToString();
	}
}