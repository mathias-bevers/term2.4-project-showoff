using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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

	public override void Awake()
	{
		base.Awake();
		DontDestroyOnLoad(gameObject);
	}


	public void Update()
	{
		if (!isRunning) { return; }

		ProcessNewClients();
		ProcessBadClients();
		ProcessExistingClients();

		if (receivedPackets.Count < 1) { return; }

		for (int i = receivedPackets.Count - 1; i >= 0; --i)
		{
			ReceivedPacket receivedPacket = receivedPackets[i];

			WriteToOthers(receivedPacket.sender, receivedPacket.AsPacket());
			receivedPackets.RemoveAt(i);
		}
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
			TcpClient client = listener.AcceptTcpClient();
			Packet packet = new();

			if (clients.Count >= 2)
			{
				Debug.LogWarning("Refused client, server is full");
				AccessCallback rejectedCB = new(false);
				packet.Write(rejectedCB);

				try { StreamUtil.Write(client.GetStream(), packet.GetBytes()); }
				catch (Exception e) { Debug.LogError(e.Message); }

				client.Close();
				continue;
			}


			++currentID;
			ServerClient serverClient = new(currentID, client);

			if (clients.Count > 0)
			{
				receivedPackets.Add(new ReceivedPacket(serverClient, new PlayerConnection(PlayerConnection.ConnectionType.Joined)));
			}

			clients.Add(serverClient);

			AccessCallback acceptedCB = new(true, currentID);
			packet.Write(acceptedCB);
			WriteToClient(serverClient, packet);

			Debug.Log($"Accepted new client with id: {currentID}");
		}
	}

	private void ProcessExistingClients()
	{
		foreach (ServerClient client in clients)
		{
			if (client.available == 0) { continue; }

			try
			{
				// new Thread(() => { }).Start();

				byte[] inBytes = StreamUtil.Read(client.stream);
				Packet packet = new(inBytes);
				SeverObject obj = packet.ReadObject();
				receivedPackets.Add(new ReceivedPacket(client, obj));
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message + $"\nAdding client {client.id} to bad clients");
				badClients.Add(client.id);
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

	private void WriteToOthers(ServerClient sender, Packet packet)
	{
		foreach (ServerClient receiver in clients)
		{
			if (receiver.id == sender.id) { continue; }

			WriteToClient(receiver, packet);
		}
	}

	private void WriteToClient(ServerClient receiver, Packet packet) { WriteToClient(receiver, packet.GetBytes()); }

	private void WriteToClient(ServerClient receiver, byte[] data)
	{
		try
		{
			StreamUtil.Write(receiver.stream, data);
			/*new Thread(() => {  }).Start();*/
		}
		catch (Exception)
		{
			Debug.Log($"Marking client#{receiver.id} as bad client");
			badClients.Add(receiver.id);
		}
	}

#if UNITY_EDITOR
	[Button("Initialize")] private void ForceInit() { Initialize(Settings.SERVER_IP, Settings.SERVER_PORT); }

	private void OnGUI() { GUI.Label(new Rect(10, 10, 160, 30), $"connected clients: {clients.Count}"); }
#endif
}