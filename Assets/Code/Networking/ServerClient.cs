using System.Net.Sockets;

namespace Code.Networking
{
	public class ServerClient
	{
		public int id { get; set; }
		public TcpClient client { get; }

		public int available => client.Available;
		public NetworkStream stream => client.GetStream();

		public ServerClient(int id, TcpClient client)
		{
			this.id = id;
			this.client = client;
		}
	}
}