using System.Net.Sockets;
using saxion_provided;

public struct ReceivedPacket
{
	public (int, TcpClient) client { get; }
	public ISerializable serializable { get; }

	public ReceivedPacket(int clientID, TcpClient tcpClient, ISerializable serializable)
	{
		client = (clientID, tcpClient);
		this.serializable = serializable;
	}
}