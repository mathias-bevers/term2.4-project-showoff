using saxion_provided;
using UnityEngine;

public class DataProcessor : MonoBehaviour
{
	private const float DISTANCE_SEND_DELAY = 5f;

	[SerializeField] private Client networkingClient;
	[SerializeField] private MapWalker walker;
	[SerializeField] private Transform ownDistanceParent;
	[SerializeField] private Transform opponentDistanceParent;
	private bool shouldUpdateOT = true;
	private bool isDeath;
	private float distTimer;

	private Transform cachedTransform;

	private void Start()
	{
		if (ReferenceEquals(ownDistanceParent, null)) { throw new UnassignedReferenceException($"{nameof(ownDistanceParent)} is not set in the editor!"); }

		if (ReferenceEquals(opponentDistanceParent, null)) { throw new UnassignedReferenceException($"{nameof(opponentDistanceParent)} is not set in the editor!"); }

		cachedTransform = transform;

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
		networkingClient.receivedDebuffEvent += OnReceivedDebuff;


		distTimer = DISTANCE_SEND_DELAY;

		ownDistanceParent.SetChildrenText("YOU: ");
		opponentDistanceParent.SetChildrenText("NO ENEMY FOUND");
	}

	private void LateUpdate()
	{
		if (isDeath) { return; }

		ownDistanceParent.SetChildrenText($"YOU: {walker.TotalMetersRan:n0}");

		if (networkingClient == null) { return; }

		if (!networkingClient.isInitialized) { return; }

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

		opponentDistanceParent.SetChildrenText($"ENEMY {dst:n0}");
	}

	private void OnOpponentConnection(PlayerConnection.ConnectionType connectionType)
	{
		shouldUpdateOT = false;
		opponentDistanceParent.SetChildrenText($"ENEMY HAS {connectionType.ToString().ToUpper()}");

		CooldownManager.Cooldown(5f, () => shouldUpdateOT = true);
	}

	private void OnPlayerDeath()
	{
		if (networkingClient == null) { return; }

		Packet packet = new();
		packet.Write(new PlayerConnection(PlayerConnection.ConnectionType.Died));
		networkingClient.SendData(packet);

		foreach (Transform child in cachedTransform) { child.gameObject.SetActive(false); }

		isDeath = true;
	}

	private void OnPowerupPickup(PickupData data)
	{
		Packet packet = new();
		packet.Write(new SendPickup(data));
		networkingClient.SendData(packet);
	}

	private void OnReceivedDebuff(PickupData data) { PickupManager.Instance.PickUpPickup(data.identifier, true); }
}