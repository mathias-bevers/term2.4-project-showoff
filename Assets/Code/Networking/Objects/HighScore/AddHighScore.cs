using saxion_provided;

public class AddHighScore : HighScoreServerObject
{
	public string name { get; private set; }
	public int score { get; private set; }
	
	public AddHighScore() { }

	public AddHighScore(string name, int score)
	{
		this.name = name;
		this.score = score;
	}


	public override void Serialize(Packet packet)
	{
		packet.Write(name);
		packet.Write(score);
	}

	public override void Deserialize(Packet packet)
	{
		name = packet.ReadString();
		score = packet.ReadInt();
	}

	public override string ToString() => name.PadRight(10) + score;
}