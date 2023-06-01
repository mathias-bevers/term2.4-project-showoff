using saxion_provided;

public class PlayerDistance : ISerializable
{
	public int id { get; set; }
	public int distance { get; set; }
	
	public void Serialize(Packet packet)
	{
		packet.Write(id);
		packet.Write(distance);
	}

	public void Deserialize(Packet packet)
	{
		id = packet.ReadInt();
		distance = packet.ReadInt();
	}
}