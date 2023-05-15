using System;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
	public static PickupManager instance { get; private set; }

	[SerializeField] private PickupData[] pickups;

	private void Awake()
	{
		if (!instance.IsNull())
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
	}

	public void SpawnPickup()
	{
		PickupData data = pickups.GetRandomElement();
		GameObject go = Instantiate(data.worldPrefab);
		Pickup pickup = go.AddComponent<Pickup>();	
	}
}

[Serializable]
public enum PickupIdentifier
{
	Slowdown,
	Speedup,
	DoublePoints,
	AddObstacle
}