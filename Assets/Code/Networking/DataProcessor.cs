using System;
using saxion_provided;
using UnityEngine;
using UnityEngine.UI;

public class DataProcessor : MonoBehaviour
{
	private const int DISTANCE_SEND_DELAY = 1;

	[SerializeField] private Client networkingClient;
	[SerializeField] private MapWalker walker;
	[SerializeField] private Text ownDistanceText;
	[SerializeField] private Text opponentDistanceText;

	private bool shouldUpdateOT = true;
	private bool isDeath;

	private float distTimer;

	private void Start()
	{
		try { networkingClient.Connect(); }
		catch (System.Net.WebException e)
		{
			Debug.LogError(e);
			return;
		}

		Player.Instance.deathEvent += OnPlayerDeath;
		PickupManager.Instance.pickedupPowerupEvent += OnSendPickup;
		networkingClient.oponnentDistanceRecievedEvent += UpdateOpponentText;
		networkingClient.connectionEvent += DisplayOpponentConnection;


		distTimer = DISTANCE_SEND_DELAY;

		// ownDistanceText.text = opponentDistanceText.text = string.Empty;
	}

	private void LateUpdate()
	{
		if (!networkingClient.isInitialized) { return; }

		if (isDeath) { return; }

		ownDistanceText.text = $"Score: {walker.TotalMetersRan:f2}";

		distTimer -= Time.deltaTime;

		if (distTimer > 0) { return; }

		distTimer = DISTANCE_SEND_DELAY;

		Packet packet = new();
		packet.Write(new PlayerDistance(walker.TotalMetersRan));
		networkingClient.SendData(packet);
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
		CooldownManager.Cooldown(.5f, () => networkingClient.Close());
	}

	private void OnSendPickup(PickupData data)
	{
		Debug.Log("Sending pickup to opponent!");
		Packet packet = new ();
		packet.Write(new SendPickup(data));
		networkingClient.SendData(packet);
	}
}