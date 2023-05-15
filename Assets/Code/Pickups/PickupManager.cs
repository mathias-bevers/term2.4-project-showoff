using System;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
	[SerializeField] private Transform spawnTarget;
	[SerializeField] private PickupData[] pickups;
	
	public static PickupManager instance { get; private set; }
	public Transform cachedTransform { get; private set; }

	private void Awake()
	{
		cachedTransform = transform;

		if (!instance.IsNull())
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
	}
	
	public void SpawnPickup(object obj = null)
	{
		PickupData pickupData = pickups.GetRandomElement();
		GameObject go = Instantiate(pickupData.worldPrefab, spawnTarget.position, quaternion.identity, cachedTransform);
		Pickup pickup = go.AddComponent<Pickup>();
		pickup.PickupEvent += () => pickupData.callback.Invoke(obj);
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