using System;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
	public event Action<Billboard> destroyingEvent;

	private readonly float[] possibleRotations =
	{
		-120.0f,
		0.0f,
		120.0f,
	};

	[field: SerializeField] public Image[] images { get; private set; }
	public string[] displayingImageNames { get; private set; }
	
	private Transform cachedTransform;

	private void Start()
	{
		cachedTransform = transform;
		
		float yRotation = cachedTransform.rotation.y + possibleRotations.GetRandomElementStruct(); 
		cachedTransform.rotation = Quaternion.Euler(0, yRotation, 0);
		
		displayingImageNames = BillboardManager.Instance.RequestSetup(this);
	}

	private void OnDestroy()
	{
		destroyingEvent?.Invoke(this);
		destroyingEvent = null;
	}
}