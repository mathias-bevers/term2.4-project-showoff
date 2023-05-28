using NaughtyAttributes;
using UnityEngine;

public class Testing : MonoBehaviour
{
	[SerializeField] private bool isClient;
	[SerializeField, ReadOnly] private bool isInitialized;

	private Client client;
	private Server server;

	[Button("Initialize")]
	public void Initialize()
	{
		if (isInitialized) { return; }

		isInitialized = true;

		if (isClient)
		{
			client = new Client();
			client.Connect(Settings.SERVER_IP, Settings.SERVER_PORT);
			return;
		}

		server = new Server(Settings.SERVER_IP, Settings.SERVER_PORT);
	}
}