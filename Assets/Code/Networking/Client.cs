using System;
using System.Net;
using System.Net.Sockets;
using NaughtyAttributes;
using saxion_provided;
using UnityEngine;
using Random = UnityEngine.Random;

public class Client : MonoBehaviour
{
	public int id { get; private set; } = -1;
	public bool isInitialized => id >= 0;

	private bool isAccepted;
	private TcpClient client;

	public void Update()
	{
		if (client == null) { return; }

		try
		{
			if (client.Available > 0) { ProcessData(StreamUtil.Read(client.GetStream())); }
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
			client.Close();
		}
	}

	private void OnDestroy()
	{
		Debug.LogWarning("Destroying client instance...");
		client?.Close();
	}

	public void Connect() => Connect(Settings.SERVER_IP, Settings.SERVER_PORT);

	public void Connect(IPAddress ip, int port, int attempts = 0)
	{
		if (isAccepted) { return; }

		if (attempts >= 5) { throw new Exception("FAILED TO CONNECT TO SERVER"); }

		try
		{
			client ??= new TcpClient();
			client.Connect(ip, port);
		}
		catch (SocketException se)
		{
			if (se.SocketErrorCode == SocketError.ConnectionRefused)
			{
				Server.Instance.Initialize(ip, port);
				Connect(ip, port, attempts + 1);
				Debug.LogWarning($"Retrying connection attempt: {attempts}");
				return;
			}

			Debug.LogError($"Failed to connect to server!\n{se.Message}");
		}
	}

	public void SendData(Packet packet)
	{
		if (client == null)
		{
			Debug.LogWarning("Cannot send data if there is no client connected");
			return;
		}

		if (packet == null)
		{
			Debug.LogWarning("Trying to send null");
			return;
		}

		Debug.Log($"Client#{id} is sending data to the server!");
		StreamUtil.Write(client.GetStream(), packet.GetBytes());
	}

	private void ProcessData(byte[] dataInBytes)
	{
		Packet packet = new(dataInBytes);
		if (!isAccepted)
		{
			AccessCallback callback = packet.Read<AccessCallback>();
			HandleAccessCallback(callback);
			return;
		}

		ISerializable serializable = packet.ReadObject();
		switch (serializable)
		{
			case PlayerDistance playerDistance:
				Debug.Log($"Received player#{playerDistance.id}'s distance of: {playerDistance.distance}");
				break;
			case HeartBeat: break;
		}
	}

	private void HandleAccessCallback(AccessCallback callback)
	{
		isAccepted = callback.accepted;
		if (!isAccepted)
		{
			Destroy(this);
			return;
		}

		id = callback.id;
	}

#if UNITY_EDITOR
	[Button("Initialize")] private void ForceInit() { Connect(Settings.SERVER_IP, Settings.SERVER_PORT); }
	[Button("Send fake data")]
	private void SetData()
	{
		PlayerDistance dst = new PlayerDistance
		{
			id = id,
			distance = Random.Range(0, 11),
		};

		Debug.Log($"Client#{dst.id} is sending a distance of: {dst.distance}");
		Packet packet = new();
		packet.Write(dst);

		SendData(packet);
	}
#endif
}