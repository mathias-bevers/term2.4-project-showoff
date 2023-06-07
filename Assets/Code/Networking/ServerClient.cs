using System.Net.Sockets;

public class ServerClient
{
	public ServerClient(int id, TcpClient client)
	{
		this.id = id;
		this.client = client;
	}

	public int id { get; set; }
	public TcpClient client { get; }

	public int available => client.Available;
	public NetworkStream stream => client.GetStream();
}