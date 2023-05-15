using System;
using NaughtyAttributes;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public event Action PickupEvent;

	private void OnTriggerEnter(Collider other)
	{
		//TODO: Add player check.
		PickupEvent?.Invoke();
		Destroy(gameObject);
	}

#if UNITY_EDITOR
	[Button("FORCE PICKUP")] private void ForcePickup() { OnTriggerEnter(null); }
#endif
}