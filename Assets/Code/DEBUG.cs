using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG : MonoBehaviour
{
	[SerializeField] private Vector2 rectSize;
	
	private Rect buttonRect;

	private void Awake()
	{
		buttonRect = new Rect(10, 10, rectSize.x, rectSize.y);
		buttonRect.y = Screen.height - (buttonRect.height + 10);
	}

	private void OnGUI()
	{
		PauseButton();
	}

	private void PauseButton()
	{
		if (!GUI.Button(buttonRect, "PAUSE"))
		{
			return;
		}
		
		Debug.Break();
	}
}
