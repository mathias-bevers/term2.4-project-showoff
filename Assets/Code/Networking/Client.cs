using System;
using System.Net;
using System.Net.Sockets;
using saxion_provided;
using UnityEngine;

public class Client : MonoBehaviour
{
	public event Action<PlayerConnection.ConnectionType> connectionEvent;
	public event Action<float> opponentDistanceReceivedEvent;
	public event Action<PickupData> receivedDebuffEvent;
	private const float MAX_TIME_BETWEEN_HEARTBEAT = 2.5f;

	public int id { get; private set; } = -1;
	public bool isInitialized => id >= 0;
	private bool isAccepted;


	private float timer;
	private TcpClient client;

	private void Start()
	{
		if (Player.Instance.client != null)
		{
			Destroy(gameObject);
			return;
		}

		Player.Instance.client = this;
	}

	public void Update()
	{
		try
		{
			if (client == null) { return; }

			if (isAccepted)
			{
				timer -= Time.deltaTime;
				if (timer < 0)
				{
					Debug.LogWarning("Server timed out, closing client...");
					Destroy(this);
					return;
				}
			}

			while (client is { Available: > 1 })
			{
				byte[] inBytes = StreamUtil.Read(client.GetStream());
				ProcessData(inBytes);
			}
		}
		catch (Exception e)
		{
			Debug.LogError(string.Concat(e.Message, '\n', e.StackTrace));
			Destroy(this);
		}
	}

	private void OnDestroy() => Close();

	private void Close()
	{
		if (client == null) { return; }

		isAccepted = false;
		client.Close();
		client = null;
		Destroy(this);
	}

	public void Connect() => Connect(Settings.SERVER_IP, Settings.SERVER_PORT);

	private void Connect(IPAddress ip, int port, int attempts = 0)
	{
		while (true)
		{
			if (isAccepted) { return; }

			if (ip == null) { throw new ArgumentNullException(nameof(ip), "ip cannot be null"); }

			if (attempts >= 5) { throw new WebException("FAILED TO CONNECT TO SERVER: too many attempts"); }

			client ??= new TcpClient();
			IAsyncResult result = client.BeginConnect(ip, port, null, null);
			bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));

			if (!success)
			{
				if (!GameSettings.IsHost) { throw new WebException("FAILED TO CONNECT TO SERVER: Server timed out..."); }


				try
				{
					Server.Instance.Initialize(ip, port);
					attempts++;
					Debug.LogWarning($"Retrying connection attempt: {attempts}");
					continue;
				}
				catch (InvalidOperationException e)
				{
					Debug.LogError(e.ToString());
					throw new WebException("FAILED TO CONNECT TO SERVER: server cannot be created!");
				}
			}

			client.EndConnect(result);
			break;
		}
	}

	public void SendData(Packet packet)
	{
		if (!isAccepted) { return; }

		if (packet == null)
		{
			Debug.LogWarning("Trying to send null");
			return;
		}

		try { StreamUtil.Write(client.GetStream(), packet.GetBytes()); }
		catch (Exception)
		{
			Debug.LogWarning("Cannot send data to closed stream.");
			Destroy(this);
		}
	}

	private void ProcessData(byte[] dataInBytes)
	{
		Packet packet = new(dataInBytes);
		ServerObject serverObject;
		try { serverObject = packet.ReadObject(); }
		catch (Exception e)
		{
			Debug.LogError("object could not be read.\n" + e.Message);
			Close();
			return;
		}

		if (!isAccepted)
		{
			if (serverObject is not AccessCallback callback) { return; }

			HandleAccessCallback(callback);
			return;
		}

		switch (serverObject)
		{
			case HeartBeat:
				timer = MAX_TIME_BETWEEN_HEARTBEAT;
				break;

			case PlayerDistance playerDistance:
				opponentDistanceReceivedEvent?.Invoke(playerDistance.distance);
				break;

			case PlayerConnection playerConnection:
				connectionEvent?.Invoke(playerConnection.connectionType);
				break;

			case SendPickup sendPickup:
				receivedDebuffEvent?.Invoke(sendPickup.data);
				break;

			case GetHighScores getHighScores:
				HighScoreManager.Instance.RewriteScoresToFile(getHighScores.scores);
				break;

			case GetFileNames fileNames:
				DataBaseCommunicator.Instance.RewriteDataBaseCache(fileNames);
				break;

			default: throw new NotSupportedException($"Cannot process ISerializable type {serverObject.GetType().Name}");
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
		SendData(HighScoreManager.Instance.LocalScoresAsPacket());
	}
}