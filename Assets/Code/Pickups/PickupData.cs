using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct PickupData
{
	[field: SerializeField] public PickupIdentifier identifier { get; private set; }
	[field: SerializeField] public bool shouldSendToServer { get; private set; }
	[field: SerializeField] public PickupEvent onPickupEvent { get; private set; }
	[field: SerializeField] public Parameters parameters { get; private set; }

	public PickupData(int identifier, Parameters parameters)
	{
		this.identifier = (PickupIdentifier)identifier;
		this.parameters = parameters;
		onPickupEvent = null;
		shouldSendToServer = false;
	}

	public override string ToString() => $"{identifier.ToString()} with parameters:\n{parameters.ToString()}";

	[Serializable]
	public struct Parameters
	{
		[field: SerializeField] public int wholeNumber { get; private set; }
		[field: SerializeField] public float decimalNumber { get; private set; }
		[field: SerializeField] public string message { get; private set; }
		[field: SerializeField] public bool toggle { get; private set; }

		public Parameters(int wholeNumber, float decimalNumber, string message, bool toggle)
		{
			this.wholeNumber = wholeNumber;
			this.decimalNumber = decimalNumber;
			this.message = message;
			this.toggle = toggle;
		}

		public override string ToString()
		{
			StringBuilder sb = new();
			sb.AppendLine("\tint: " + wholeNumber);
			sb.AppendLine("\tfloat: " + decimalNumber);
			sb.AppendLine($"\tstring: \"{message}\"");
			sb.AppendLine("\tbool: " + toggle);

			return sb.ToString();
		}
	}
}

[Serializable] public class PickupEvent : UnityEvent<PickupData.Parameters> { }