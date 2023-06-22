using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using saxion_provided;
using UnityEngine;

public class Server : Singleton<Server>
{
	private const int MAX_PLAYERS = 2;
	private bool isRunning;
	private DataBaseCloud dataBaseCloud;

	private HighScoreCloud highScoreCloud;
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

		ProcessReceivedPackets();
	}

	private void OnDestroy()
	{
		listener.Server.Close();
		// Debug.LogWarning("Closing server!");
	}

	private void ProcessReceivedPackets()
	{
		if (receivedPackets.Count < 1) { return; }

		for (int i = receivedPackets.Count - 1; i >= 0; --i)
		{
			ReceivedPacket receivedPacket = receivedPackets[i];

			switch (receivedPacket.serverObject)
			{
				case HighScoreServerObject asHighScore:
					try { highScoreCloud.ProcessPacket(receivedPacket.sender, asHighScore); }
					catch (ArgumentException e) { Debug.LogError(e); }
					break;
				
				case DataBaseObject dataBaseObject:
					try { dataBaseCloud.ProcessPacket(receivedPacket.sender, dataBaseObject); }
					catch (Exception e) { Debug.Log(e); }
					break;
				
				default: WriteToOthers(receivedPacket.sender, receivedPacket.AsPacket());
					break;
			}

			receivedPackets.RemoveAt(i);
		}
	}

	public void Initialize(IPAddress ip, int port)
	{
		listener = new TcpListener(ip, port);
		listener.Start();

		clients = new List<ServerClient>(MAX_PLAYERS);
		badClients = new List<int>();
		receivedPackets = new List<ReceivedPacket>();
		highScoreCloud = new HighScoreCloud(this);
		dataBaseCloud = new DataBaseCloud(this);

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

			if (clients.Count > 0) { receivedPackets.Add(new ReceivedPacket(serverClient, new PlayerConnection(PlayerConnection.ConnectionType.Joined))); }

			clients.Add(serverClient);

			AccessCallback acceptedCB = new(true, currentID);
			packet.Write(acceptedCB);
			WriteToClient(serverClient, packet);

			//Debug.Log($"Accepted new client with id: {currentID}");
		}
	}

	private void ProcessExistingClients()
	{
		foreach (ServerClient client in clients)
		{
			if (client.available == 0) { continue; }

			try
			{
	
					byte[] inBytes = StreamUtil.Read(client.stream);
					Packet packet = new(inBytes);
					ServerObject obj = packet.ReadObject();
					receivedPackets.Add(new ReceivedPacket(client, obj));
			
			}
			catch (Exception)
			{
				//Debug.LogError(e.Message + $"\nAdding client {client.id} to bad clients");
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


	public void WriteToClient(ServerClient receiver, Packet packet) { WriteToClient(receiver, packet.GetBytes()); }

	private void WriteToClient(ServerClient receiver, byte[] data)
	{
		try { StreamUtil.Write(receiver.stream, data); }
		catch (Exception)
		{
			//Debug.Log($"Marking client#{receiver.id} as bad client");
			badClients.Add(receiver.id);
		}
	}

	public string DEBUG_INFO()
	{
		StringBuilder sb = new("SERVER DEBUG:\n\n");
		sb.AppendLine($"Connected clients: {clients.Count}");
		sb.AppendLine($"Package backlog: {receivedPackets.Count}");

		return sb.ToString();
	}
}