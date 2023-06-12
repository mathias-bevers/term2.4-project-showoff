using saxion_provided;

public class AccessCallback : ServerObject
{
	public AccessCallback(bool accepted, int id = -1)
	{
		this.accepted = accepted;
		this.id = id;
	}

	public AccessCallback() { }

	public bool accepted { get; private set; }
	public int id { get; private set; }

	public override void Serialize(Packet packet)
	{
		packet.Write(accepted);

		if (!accepted) { return; }

		packet.Write(id);
	}

	public override void Deserialize(Packet packet)
	{
		accepted = packet.ReadBool();

		if (!accepted) { return; }

		id = packet.ReadInt();
	}
}