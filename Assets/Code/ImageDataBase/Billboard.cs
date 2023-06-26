using System;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
	public event Action<Billboard> destroyingEvent;
	
	[field: SerializeField] public new MeshRenderer renderer { get; private set; }
	public string displayingImageName { get; private set; }
	
	private void Start()
	{
		if (!renderer.enabled) { return; }

		if (ReferenceEquals(renderer, null)) { throw new UnassignedReferenceException($"{nameof(renderer)} is not set in the editor!"); }
		
		displayingImageName = BillboardManager.Instance.RequestSetup(this);
	}
	
	private void OnDestroy()
	{
		destroyingEvent?.Invoke(this);
		destroyingEvent = null;
	}
}