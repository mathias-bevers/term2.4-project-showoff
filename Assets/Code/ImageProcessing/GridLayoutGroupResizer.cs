using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutGroupResizer : MonoBehaviour
{
	private const float RESIZE_DELAY = 0.5f;

	private RectTransform canvas;
	private float timer;
	private GridLayoutGroup glg;
	private RectTransform cachedRectTransform;
	private Vector2 currentCanvasSize;
	private Vector2 canvasDesignSize;
	private Vector2 itemDesignPercent;
	private Vector2 gapDesignPercent;

	private void Awake()
	{
		cachedRectTransform = transform as RectTransform;
		glg = gameObject.GetComponentThrow<GridLayoutGroup>();
		canvas = (RectTransform)cachedRectTransform.GetComponentInParents<Canvas>().transform;
		CanvasScaler canvasScaler = canvas.gameObject.GetComponentThrow<CanvasScaler>();

		canvasDesignSize = canvasScaler.referenceResolution;
		itemDesignPercent = new Vector2(glg.cellSize.x / canvasDesignSize.x, glg.cellSize.y / canvasDesignSize.y);
		gapDesignPercent = new Vector2(glg.spacing.x / canvasDesignSize.x, glg.spacing.y / canvasDesignSize.y);
	}

	private void Update()
	{
		timer -= Time.deltaTime;

		if (timer > 0) { return; }

		timer = RESIZE_DELAY;

		if (canvas.rect.size == currentCanvasSize) { return; }

		Resize();
	}

	[Button]
	private void Resize()
	{
		Vector2 canvasSize = canvas.rect.size;

		Vector2 newCellSize = new(canvasSize.x * itemDesignPercent.x, canvasSize.y * itemDesignPercent.y);
		glg.cellSize = newCellSize;

		Vector2 newGapSize = new(canvasSize.x * gapDesignPercent.x, canvasSize.y * gapDesignPercent.y);
		glg.spacing = newGapSize;
		

		currentCanvasSize = canvasSize;
	}
}