using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using saxion_provided;
using Debug = UnityEngine.Debug;

public struct ReceivedPacket
{
	public ServerClient sender { get; }
	public SeverObject severObject { get; }

	public ReceivedPacket(ServerClient sender, SeverObject severObject)
	{
		this.sender = sender;
		this.severObject = severObject;
	}

	public Packet AsPacket()
	{
		if (severObject == null)
		{
			StringBuilder sb = new("Trying to write an empty package!\n");
		
			StackTrace st = new(true);
			for (int i = 0; i < st.FrameCount; i++)
			{
				StackFrame sf = st.GetFrame(i);
				string fileName = sf.GetFileName().Split('\\').Last();
				sb.Append(fileName.PadLeft(5 * i));
				sb.Append(" at line ");
				sb.AppendLine(sf.GetFileLineNumber().ToString());
			}
		
			throw new NoNullAllowedException(sb.ToString());
		}
		
		Packet packet = new();
		packet.Write(severObject);
		return packet;
	}
}