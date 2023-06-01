using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Code.Networking;
using NaughtyAttributes;
using saxion_provided;
using UnityEngine;

public class Server : Singleton<Server>
{
	private const int MAX_PLAYERS = 2;
	private bool isRunning;

	private int currentID = -1;
	private List<int> badClients;
	private List<ReceivedPacket> receivedPackets;
	private List<ServerClient> clients;

	private TcpListener listener;


	public void Update()
	{
		if (!isRunning) { return; }

		ProcessNewClients();
		ProcessBadClients();
		ProcessExistingClients();
	}

	public void Initialize(IPAddress ip, int port)
	{
		clients = new List<ServerClient>(MAX_PLAYERS);
		badClients = new List<int>();
		receivedPackets = new List<ReceivedPacket>();

		listener = new TcpListener(ip, port);
		listener.Start();
		isRunning = true;

		Debug.Log($"Started new server on <b>{listener.LocalEndpoint}</b>!");
	}


	private void ProcessNewClients()
	{
		while (listener.Pending())
		{
			Packet packet = new();
			AccessCallback callback;
			TcpClient client = listener.AcceptTcpClient();

			if (clients.Count >= 2)
			{
				Debug.LogWarning("Refused client, server is full");
				callback = new AccessCallback(false);
				packet.Write(callback);
				
				try { StreamUtil.Write(client.GetStream(), packet.GetBytes()); }
				catch (System.IO.IOException e) { }

				client.Close();
				continue;
			}


			++currentID;
			ServerClient serverClient = new (currentID, client);
			clients.Add(serverClient);
			
			callback = new AccessCallback(true, currentID);
			packet.Write(callback);
			WriteToClient(serverClient, packet);
			
			Debug.Log($"Accepted new client with id: {currentID}");
		}
	}

	private void ProcessExistingClients()
	{
		foreach (ServerClient client in clients)
		{
			if (client.available == 0) { continue; }

			Debug.Log($"Server received data from Client#{client.id}");
			byte[] inBytes = StreamUtil.Read(client.stream);

			foreach (ServerClient receiver in clients)
			{
				if (receiver.id == client.id) { continue; }

				WriteToClient(receiver, inBytes);
			}
		}
	}

	private void ProcessBadClients()
	{
		foreach (ServerClient client in clients)
		{
			Packet packet = new();
			packet.Write(new HeartBeat());
			WriteToClient(client, packet);
		}

		if (badClients.IsNullOrEmpty()) { return; }

		foreach (int badClientID in badClients)
		{
			ServerClient badClient = clients.Find(client => client.id == badClientID);
			clients.Remove(badClient);
		}

		badClients.Clear();
	}

	private void WriteToClient(ServerClient receiver, Packet packet) { WriteToClient(receiver, packet.GetBytes()); }

	private void WriteToClient(ServerClient receiver, byte[] data)
	{
		try { StreamUtil.Write(receiver.stream, data); }
		catch (System.IO.IOException)
		{
			Debug.Log($"Marking client#{receiver.id} as bad client");
			badClients.Add(receiver.id);
		}
	}

#if UNITY_EDITOR
	[Button("Initialize")] private void ForceInit() { Initialize(Settings.SERVER_IP, Settings.SERVER_PORT); }
#endif
}