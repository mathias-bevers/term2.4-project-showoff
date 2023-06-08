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


		foreach (PickupData data in pickups)
		{
			if (data.identifier != helper.pickupType) { continue; }

			Debug.Log($"Picked up {data} ");
			if (data.shouldSendToServer) { pickedupPowerupEvent?.Invoke(data); }
			else { data.onPickupEvent?.Invoke(data.parameters); }
		}

		helper.isPendingDestroy = true;
		Destroy(hit.transform.gameObject);
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