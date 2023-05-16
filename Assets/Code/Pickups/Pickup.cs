using System;
using NaughtyAttributes;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public event Action onPickupEvent;

	private void OnDestroy() { onPickupEvent = null; }

	private void OnTriggerEnter(Collider other) { onPickupEvent?.Invoke(); }
}