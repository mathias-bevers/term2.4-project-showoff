using System;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
	public static PickupManager instance { get; private set; }

	private void Awake()
	{
		if (!instance.IsNull())
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
	}
	
	
}
