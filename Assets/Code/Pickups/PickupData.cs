using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct PickupData
{
	[field: SerializeField] public PickupIdentifier identifier { get; private set; }
	[field: SerializeField] public GameObject worldPrefab { get; private set; }
	[field: SerializeField] public PickupEvent onPickupEvent { get; private set; }
	[field: SerializeField] public Parameters parameters { get; private set; }

	[Serializable]
	public struct Parameters
	{
		[field: SerializeField] public int wholeNumber { get; private set; }
		[field: SerializeField] public float decimalNumber { get; private set; }
		[field: SerializeField] public string message { get; private set; }
		[field: SerializeField] public bool toggle { get; private set; }
	}
}

[Serializable]
public class PickupEvent : UnityEvent<PickupData.Parameters> { }