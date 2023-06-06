using saxion_provided;
using UnityEngine;
using UnityEngine.UI;

public class BUILD_TEST : MonoBehaviour
{
	[SerializeField] private Button connectButton;
	[SerializeField] private Button sendDataButton;
	[SerializeField] private Client client;

	private void Awake()
	{
		Screen.SetResolution(720, 480, FullScreenMode.Windowed);

		connectButton.onClick.AddListener(client.Connect);
		sendDataButton.onClick.AddListener(SendFakeData);
	}

	private void SendFakeData()
	{
		if (client == null || !client.isInitialized) { Debug.LogError("Client is not ready to send data"); }

		int distance = Random.Range(0, 1001);
		PlayerDistance playerDistance = new(distance);

		Packet packet = new();
		packet.Write(playerDistance);
		client.SendData(packet);
	}
}