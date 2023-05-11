using System;
using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
	[Flags] public enum Target { Self = 1, Opponent = 1 << 1 }

	[field: SerializeField] public Target target { get; private set; } = Target.Opponent;
	[SerializeField] public UnityEvent onPickup;

	private void OnTriggerEnter(Collider other)
	{
		//TODO: add player check
		onPickup.Invoke();
	}
}