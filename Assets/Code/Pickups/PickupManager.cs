using UnityEngine;
using System;

public class PickupManager : Singleton<PickupManager>
{
	[SerializeField] private PickupData[] pickups;

	[NaughtyAttributes.Button("Spawn pickup")]
	public void SpawnPickup()
	{
		PickupData data = pickups.GetRandomElementStruct();
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