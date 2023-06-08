using saxion_provided;

public class SendPickup : ServerObject
{
	public SendPickup() { }
	public SendPickup(PickupData data) { this.data = data; }
	
	public PickupData data { get; private set; }

	public override void Serialize(Packet packet)
	{
		packet.Write((int)data.identifier);

		// Write the parameters.
		packet.Write(data.parameters.wholeNumber);
		packet.Write(data.parameters.decimalNumber);
		packet.Write(data.parameters.message);
		packet.Write(data.parameters.toggle);
	}

	public override void Deserialize(Packet packet)
	{
		int identifier = packet.ReadInt();

		// Read the parameters.
		int wholeNumber = packet.ReadInt();
		float decimalNumber = packet.ReadFloat();
		string message = packet.ReadString();
		bool toggle = packet.ReadBool();

		// Create a new pickup data instance 
		PickupData.Parameters parameters = new(wholeNumber, decimalNumber, message, toggle);
		data = new PickupData(identifier, parameters);
	}
}