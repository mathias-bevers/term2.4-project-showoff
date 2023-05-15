using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct PickupData
{
	[field: SerializeField] public PickupIdentifier identifier { get; private set; }
	[field: SerializeField] public GameObject worldPrefab { get; private set; }
	[field: SerializeField] public ObjectEvent onPickupEvent { get; private set; }
}

[Serializable]
public class ObjectEvent : UnityEvent<object> { }