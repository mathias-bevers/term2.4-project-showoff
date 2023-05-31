using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using NaughtyAttributes;
using saxion_provided;
using UnityEngine;

public class Server : Singleton<Server>
{
	private bool isRunning;
	private int currentID = -1;
	private List<TcpClient> clients;
	private TcpListener listener;


	public void Update()
	{
		if (!isRunning) { return; }

		ProcessNewClients();
		ProcessExistingClients();
	}

	public void Initialize(IPAddress ip, int port)
	{
		listener = new TcpListener(ip, port);
		clients = new List<TcpClient>(2);
		listener.Start();

		isRunning = true;

		Debug.Log("Started new server!");
	}


	private void ProcessNewClients()
	{
		while (listener.Pending())
		{
			Packet packet = new();

			if (clients.Count >= 2)
			{
				TcpClient rejected = listener.AcceptTcpClient();
				packet.Write(false);
				WriteToClient(rejected, packet);
				rejected.Close();
				Debug.LogWarning("Refused client, server is full");
				continue;
			}

			TcpClient accepted = listener.AcceptTcpClient();
			clients.Add(accepted);
			++currentID;

			packet.Write(true);
			packet.Write(currentID);
			WriteToClient(accepted, packet);
			Debug.Log($"Accepted new client with id: {currentID}");
		}
	}

	private void ProcessExistingClients()
	{
		foreach (TcpClient client in clients)
		{
			if (client.Available == 0) { continue; }

			byte[] inBytes = StreamUtil.Read(client.GetStream());
		}
	}

	private void WriteToClient(TcpClient receiver, Packet data)
	{
		//TODO: add try catch etc
		StreamUtil.Write(receiver.GetStream(), data.GetBytes());
	}

#if UNITY_EDITOR
	[Button("Initialize")] private void ForceInit() { Initialize(Settings.SERVER_IP, Settings.SERVER_PORT); }
#endif
}