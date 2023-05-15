using System;
using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
	public event Action onPickupEvent;
	
	private void OnTriggerEnter(Collider other)
	{
		onPickupEvent.Invoke();
	}
}