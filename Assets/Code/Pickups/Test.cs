using System;
using UnityEngine;

public class Test : MonoBehaviour
{
	public void ObjectTestMethod(object testParameter) { Debug.Log($"Test method: {testParameter}!"); }

	private void Update()
	{
		if(!Input.GetKeyDown(KeyCode.Space)) {return;}
		SpawnObject();
	}

	private void SpawnObject()
	{
		PickupManager.instance.SpawnPickup(42);
	}
}