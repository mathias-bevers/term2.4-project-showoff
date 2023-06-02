using saxion_provided;
using UnityEngine;
using UnityEngine.UI;

public class DataProcessor : MonoBehaviour
{
	[SerializeField] private Client networkingClient;
	[SerializeField] private MapWalker walker;
	[SerializeField] private Text ownDistanceText;
	[SerializeField] private Text opponentDistanceText;
	[SerializeField] private GameObject debugConsole;

	private bool shouldUpdateOT = true;
	private bool isDeath;

	private void Start()
	{
		DontDestroyOnLoad(debugConsole);
		
		networkingClient.Connect();

		Player.Instance.deathEvent += OnPlayerDeath;
		networkingClient.oponnentDistanceRecievedEvent += UpdateOpponentText;
		networkingClient.connectionEvent += DisplayOpponentConnection;

		// ownDistanceText.text = opponentDistanceText.text = string.Empty;
	}

	private void LateUpdate()
	{
		if (!networkingClient.isInitialized)
		{
			return;
		}
		if (isDeath) { return; }

		Packet packet = new();
		packet.Write(new PlayerDistance(walker.TotalMetersRan));
		networkingClient.SendData(packet);
		
		ownDistanceText.text = $"Score: {walker.TotalMetersRan:f2}";
	}

	private void OnDestroy()
	{
		if (isDeath) { return; }

		Packet packet = new();
		packet.Write(new PlayerConnection(PlayerConnection.ConnectionType.Left));
		networkingClient.SendData(packet);
	}

	private void UpdateOpponentText(float dst)
	{
		if (!shouldUpdateOT) { return; }

		opponentDistanceText.text = $"Opponent: {dst:f2}";
	}

	private void DisplayOpponentConnection(PlayerConnection.ConnectionType connectionType)
	{
		shouldUpdateOT = false;
		opponentDistanceText.text = $"Opponent has: {connectionType.ToString()}";
		CooldownManager.Cooldown(5f, () => shouldUpdateOT = true);
	}

	private void OnPlayerDeath()
	{
		Packet packet = new();
		packet.Write(new PlayerConnection(PlayerConnection.ConnectionType.Died));
		networkingClient.SendData(packet);

		ownDistanceText.transform.parent.gameObject.SetActive(false);
		
		isDeath = true;
		networkingClient.Close();
	}
}