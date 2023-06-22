using System;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
	public event Action<Billboard> destroyingEvent;


	[field: SerializeField] public Image[] images { get; private set; }
	public string[] displayingImageNames { get; private set; }
	
	private void Start() { displayingImageNames = BillboardManager.Instance.RequestSetup(this); }

	private void OnDestroy()
	{
		destroyingEvent?.Invoke(this);
		destroyingEvent = null;
	}
}