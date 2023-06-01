using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using NaughtyAttributes;
using saxion_provided;
using UnityEngine;

public class Server : Singleton<Server>
{
	private bool isRunning;

	private Dictionary<int, TcpClient> clients;
	private int currentID = -1;
	private List<int> badClients;

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
		listener = new TcpListener(ip, port);
		clients = new Dictionary<int, TcpClient>(2);
		badClients = new List<int>();
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

			if (clients.Count >= 2)
			{
				Debug.LogWarning("Refused client, server is full");
				TcpClient rejected = listener.AcceptTcpClient();
				callback = new AccessCallback(false);
				packet.Write(callback);
				WriteToClient((-1, rejected), packet);
				rejected.Close();
				continue;
			}

			TcpClient accepted = listener.AcceptTcpClient();
			++currentID;
			clients.Add(currentID, accepted);

			callback = new AccessCallback(true, currentID);
			packet.Write(callback);
			WriteToClient((currentID, accepted), packet);
			Debug.Log($"Accepted new client with id: {currentID}");
		}
	}

	private void ProcessExistingClients()
	{
		foreach (KeyValuePair<int, TcpClient> idClient in clients)
		{
			if (idClient.Value.Available == 0) { continue; }

			Debug.Log($"Client#{idClient.Key} is sending data");
			byte[] inBytes = StreamUtil.Read(idClient.Value.GetStream());

			foreach (KeyValuePair<int, TcpClient> idReceiver in clients)
			{
				if (idReceiver.Key == idClient.Key) { continue; }

				WriteToClient((idClient.Key, idReceiver.Value), inBytes);
			}
		}
	}

	private void ProcessBadClients()
	{
		foreach (KeyValuePair<int, TcpClient> idClient in clients)
		{
			Packet packet = new();
			packet.Write(new HeartBeat());
			WriteToClient((idClient.Key, idClient.Value), packet);
		}

		if (badClients.IsNullOrEmpty()) { return; }

		foreach (int badClientID in badClients) { clients.Remove(badClientID); }

		badClients.Clear();
	}

	private void WriteToClient((int, TcpClient) idReceiver, Packet packet) { WriteToClient(idReceiver, packet.GetBytes()); }

	private void WriteToClient((int, TcpClient) idReceiver, byte[] data)
	{
		try { StreamUtil.Write(idReceiver.Item2.GetStream(), data); }
		catch (System.IO.IOException)
		{
			Debug.Log($"Marking client#{idReceiver.Item1} as bad client");
			badClients.Add(idReceiver.Item1);
		}
	}

#if UNITY_EDITOR
	[Button("Initialize")] private void ForceInit() { Initialize(Settings.SERVER_IP, Settings.SERVER_PORT); }
#endif
}