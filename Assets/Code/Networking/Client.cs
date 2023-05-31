using System;
using System.Net;
using System.Net.Sockets;
using NaughtyAttributes;
using saxion_provided;
using UnityEngine;

public class Client : MonoBehaviour
{
	private bool? isAccepted;
	private int id;
	private TcpClient client;

	public void Update()
	{
		if (client == null) { return; }

		if (isAccepted.HasValue && !isAccepted.Value) { return; }

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

	public void Connect(IPAddress ip, int port, int attempts = 0)
	{
		if (isAccepted.HasValue) { return; }

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
				Server.Instance.Initialize(Settings.SERVER_IP, Settings.SERVER_PORT);
				Connect(Settings.SERVER_IP, Settings.SERVER_PORT, attempts + 1);
				Debug.LogWarning($"Retrying connection attempt: {attempts}");
				return;
			}

			Debug.LogError($"Failed to connect to server!\n{se.Message}");
		}
	}

	private void ProcessData(byte[] dataInBytes)
	{
		Packet packet = new(dataInBytes);
		if (!isAccepted.HasValue)
		{
			isAccepted = packet.ReadBool();
			if (!isAccepted.Value)
			{
				Destroy(this);
				return;
			}

			id = packet.ReadInt();
			return;
		}

		throw new NotImplementedException();
	}

#if UNITY_EDITOR
	[Button("Initialize")] private void ForceInit() { Connect(Settings.SERVER_IP, Settings.SERVER_PORT); }
#endif
}