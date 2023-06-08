using saxion_provided;

public class PlayerConnection : ServerObject
{
	public enum ConnectionType
	{
		Joined, Died, Left,
	}

	public ConnectionType connectionType;

	public PlayerConnection(ConnectionType connectionType) { this.connectionType = connectionType; }
	public PlayerConnection() { }

	public override void Serialize(Packet packet) { packet.Write((int)connectionType); }

	public override void Deserialize(Packet packet) { connectionType = (ConnectionType)packet.ReadInt(); }
}