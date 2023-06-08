using System;
using UnityEngine;

public class PickupManager : Singleton<PickupManager>
{
	public event Action<PickupData> pickedupPowerupEvent;
	[SerializeField] private PickupData[] pickups;

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		PickupHelper helper = hit.transform.GetComponent<PickupHelper>();
		if (helper == null) { return; }

		if (helper.isPendingDestroy) { return; }

		PickUpPickup(helper.pickupType);
		

		helper.isPendingDestroy = true;
		Destroy(hit.transform.gameObject);
	}

	public void PickUpPickup(PickupIdentifier pickupType, bool hasReceivedFromServer)
	{
		foreach (PickupData pickup in pickups)
		{
			if (pickup.identifier != pickupType) { continue; }

			// Debug.Log($"Picked up {data} ");
			if (pickup.shouldSendToServer && !hasReceivedFromServer) { pickedupPowerupEvent?.Invoke(pickup); }
			else { pickup.onPickupEvent?.Invoke(pickup.parameters); }
		}
	}
}

[Serializable]
public enum PickupIdentifier
{
	Slowdown,
	Speedup,
	DoublePoints,
	AddObstacle,
	AddHeart,
	RemoveHeart,
	SpeedupRework,
	SlowdownRework,
	Slippery,
	Invincible,
}