using System.Collections.Generic;
using System.Linq;
using saxion_provided;

public class AddHighScores : HighScoreServerObject
{
	public List<(string, int)> scores { get; }

	public AddHighScores() { scores = new List<(string, int)>(); }

	public AddHighScores(IEnumerable<(string, int)> scores)
	{
		this.scores = (scores == null) ? new List<(string, int)>() : scores.ToList();
	}

	public override void Serialize(Packet packet)
	{
		packet.Write(scores.Count);

		for (int i = 0; i < scores.Count; ++i)
		{
			packet.Write(scores[i].Item1);
			packet.Write(scores[i].Item2);
		}
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
	}
}