using saxion_provided;

public class AddFileName : DataBaseObject
{
	public string fileName { get; private set; }

	public AddFileName() { fileName = string.Empty; }
	public AddFileName(string fileName) { this.fileName = fileName; }

	public override void Serialize(Packet packet) { packet.Write(fileName); }

	public override void Deserialize(Packet packet) { fileName = packet.ReadString(); }
}