using System;
using UnityEngine;
using UnityEngine.UI;

public class DataBaseDisplayElement : MonoBehaviour
{
	[field: SerializeField] public Image imageToDisplay { get; private set; }
	public Selectable selectable { get; private set; }
	private Navigation originalNavigation;
	
	public void SetImage(Sprite sprite)
	{
		if (ReferenceEquals(sprite, null)) { throw new ArgumentNullException(nameof(sprite), "cannot be null"); }

		imageToDisplay.sprite = sprite;
	}

	public void ResetNavigation()
	{
		if (selectable == null)
		{
			selectable = this.GetComponentThrow<Selectable>();
			originalNavigation = selectable.navigation;
			return;
		}

		selectable.navigation = originalNavigation;
	}
}