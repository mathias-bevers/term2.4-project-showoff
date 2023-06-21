using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MemeDataBase : MonoBehaviour
{
	private readonly string[] acceptedFileFormats =
	{
		"jpg",
		"jpeg",
		"png",
		"tiff",
		"bmp",
	};

	[SerializeField] private int imagesPerRow;
	[SerializeField] private int rows;
	[SerializeField] private Transform content;

	private Dictionary<string, Sprite> spriteCache;
	private Image[] uiImages;
	private int scrollIndex;
	private int gridSize;
	private int pages;
	private string imageDirectoryPath;
	private string[] fileNames;

	private void Awake()
	{
		spriteCache = new Dictionary<string, Sprite>();
		gridSize = imagesPerRow * rows;
		uiImages = GetUIImages();
		imageDirectoryPath = string.Concat(Application.streamingAssetsPath, Path.DirectorySeparatorChar, "BillboardImages", Path.DirectorySeparatorChar);
		fileNames = acceptedFileFormats.SelectMany(fileFormat => Directory.GetFiles(imageDirectoryPath, $"*.{fileFormat}", SearchOption.AllDirectories)).ToArray();
		pages = (int)Math.Ceiling((float)fileNames.Length / gridSize);

		DisplayImages();
		EventSystem.current.SetSelectedGameObject(uiImages[0].transform.parent.gameObject);
	}

	private Sprite[] LoadSprites()
	{
		int start = gridSize * scrollIndex;
		int max = start + gridSize;
		int end = Math.Min(max, fileNames.Length);

		List<Sprite> sprites = new(imagesPerRow * rows);

		for (int i = start; i < end; ++i)
		{
			string fileName = fileNames[i];

			if (spriteCache.TryGetValue(fileName, out Sprite sprite))
			{
				sprites.Add(sprite);
				continue;
			}

			byte[] inBytes = File.ReadAllBytes(fileName);
			Texture2D texture = new(1, 1);
			texture.LoadImage(inBytes);

			sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			sprites.Add(sprite);
			spriteCache.Add(fileName, sprite);
		}

		return sprites.ToArray();
	}

	private void DisplayImages()
	{
		Sprite[] loaded = LoadSprites();
		for (int i = 0; i < uiImages.Length; ++i)
		{
			Image uiImage = uiImages[i];

			if (i >= loaded.Length)
			{
				uiImage.transform.parent.gameObject.SetActive(false);
				continue;
			}

			uiImage.transform.parent.gameObject.SetActive(true);
			uiImage.sprite = loaded[i];
		}
	}

	public void Scroll(int amount)
	{
		scrollIndex += amount;
		scrollIndex %= pages;
		
		DisplayImages();
	}

	private Image[] GetUIImages()
	{
		List<Image> images = new();
		foreach (Transform child in content)
		{
			if (!child.GetChild(0).TryGetComponent(out Image image)) { Debug.LogError($"All children in {content.name} need have a {nameof(Image)} component."); }

			images.Add(image);
		}

		return images.ToArray();
	}

	private void SelectImage(int gridIndex)
	{
		
	}
}