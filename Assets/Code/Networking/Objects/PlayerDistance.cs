using saxion_provided;

public class PlayerDistance : ServerObject
{
	public PlayerDistance() { }
	public PlayerDistance(float distance) { this.distance = distance; }

	public float distance { get; private set; }

	public override void Serialize(Packet packet) { packet.Write(distance); }

	public override void Deserialize(Packet packet) { distance = packet.ReadFloat(); }
}