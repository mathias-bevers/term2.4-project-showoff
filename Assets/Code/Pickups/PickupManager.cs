using System;
using NaughtyAttributes;
using UnityEngine;

public class PickupManager : Singleton<PickupManager>
{
	[SerializeField] private PickupData[] pickups;

	[Button("Spawn pickup")]
	public void SpawnPickup()
	{
		PickupData data = pickups.GetRandomElement();
		GameObject go = Instantiate(data.worldPrefab);
		Pickup pickup = go.AddComponent<Pickup>();
		pickup.onPickupEvent += () =>
		{
			data.onPickupEvent.Invoke(data.parameters);
			
			
		};
	}
}

[Serializable]
public enum PickupIdentifier
{
	Slowdown,
	Speedup,
	DoublePoints,
	AddObstacle,
}