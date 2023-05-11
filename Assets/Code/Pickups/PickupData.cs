using System;
using UnityEngine;
using UnityEngine.Events;

[Flags] public enum TargetType { Self = 1, Opponent = 2 }

[CreateAssetMenu(fileName = "PickupData", menuName = "Showoff/PickupData", order = 0)]
public class PickupData : ScriptableObject
{
	[field: SerializeField] public TargetType targetType { get; private set; }
	[field: SerializeField] public UnityEvent onPickup { get; private set; }
}