using System;
using saxion_provided;
using UnityEngine;
using UnityEngine.UI;

public class DataProcessor : MonoBehaviour
{
	private const float DISTANCE_SEND_DELAY = 0.5f;

	[SerializeField] private Client networkingClient;
	[SerializeField] private MapWalker walker;
	[SerializeField] private Text ownDistanceText;
	[SerializeField] private Text opponentDistanceText;

	private bool shouldUpdateOT = true;
	private bool isDeath;

	private float distTimer;

	private void Start()
	{
		try
		{
			#if UNITY_EDITOR
			networkingClient.Connect(Utils.GetIP4Address(), Settings.SERVER_PORT);
			#else
			networkingClient.Connect(); 
			#endif
		}
		catch (System.Net.WebException e)
		{
			Debug.LogError(e);
			return;
		}

		Player.Instance.deathEvent += OnPlayerDeath;
		PickupManager.Instance.pickedupPowerupEvent += OnPowerupPickup;

		networkingClient.opponentDistanceReceivedEvent += OnReceivedOpponentDistance;
		networkingClient.connectionEvent += OnOpponentConnection;
		networkingClient.receivedDefbuffEvent += OnReceivedDebuff;


		distTimer = DISTANCE_SEND_DELAY;

		ownDistanceText.text = "Score: ";
		opponentDistanceText.text = "NO OPPONENT FOUND";
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

	private void OnReceivedOpponentDistance(float dst)
	{
		if (!shouldUpdateOT) { return; }

		opponentDistanceText.text = $"Opponent: {dst:f2}";
	}

	private void OnOpponentConnection(PlayerConnection.ConnectionType connectionType)
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

	private void OnPowerupPickup(PickupData data)
	{
		Packet packet = new();
		packet.Write(new SendPickup(data));
		networkingClient.SendData(packet);
	}

	private void OnReceivedDebuff(PickupData data)
	{
		Debug.Log($"Received debuff: {data}");
		PickupManager.Instance.PickUpPickup(data.identifier, true);
	}
}