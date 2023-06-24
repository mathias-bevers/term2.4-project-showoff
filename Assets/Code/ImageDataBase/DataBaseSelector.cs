using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DataBaseSelector : MonoBehaviour
{
	private const string DIRECTORY_NAME = "BillboardImages";
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
	[SerializeField] private Button previousPage;
	[SerializeField] private Button nextPage;

	public Dictionary<string, Sprite> spriteCache { get; private set; }
	public string imageDirectoryPath { get; private set; }
	public event Action<string> imageSelectedEvent; 

	private Image[] uiImages;
	private int scrollIndex;
	private int gridSize;
	private int pages;
	private string[] fileNames;

	private void Awake()
	{
		spriteCache = new Dictionary<string, Sprite>();
		gridSize = imagesPerRow * rows;
		imageDirectoryPath = string.Concat(Application.streamingAssetsPath, Path.DirectorySeparatorChar, DIRECTORY_NAME, Path.DirectorySeparatorChar);
		fileNames = GetFileNames();
		pages = (int)Math.Ceiling((float)fileNames.Length / gridSize);

		SetupGrid();
		DisplayImages();

		nextPage.onClick.AddListener(() => Scroll(1));
		previousPage.onClick.AddListener(() => Scroll(-1));

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
			
			sprite = Utils.LoadSpriteFromDisk(imageDirectoryPath + fileName);
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
			GameObject imageParent = uiImage.transform.parent.gameObject;

			if (i >= loaded.Length)
			{
				imageParent.SetActive(false);
				continue;
			}

			imageParent.SetActive(true);
			uiImage.sprite = loaded[i];
		}

		SetNavigation(loaded.Length);
	}

	public void Scroll(int amount)
	{
		scrollIndex += amount;
		scrollIndex %= pages;

		if (scrollIndex < 0) { scrollIndex = pages - 1; }

		DisplayImages();
	}

	private void SetupGrid()
	{
		List<Image> images = new();

		for (int i = 0; i < content.childCount; ++i)
		{
			Transform imageBorder = content.GetChild(i);

			if (!imageBorder.TryGetComponent(out Button button)) { Debug.Log($"All children in {content.name} need to have a {nameof(Button)} component"); }

			if (!imageBorder.GetChild(0).TryGetComponent(out Image image)) { Debug.LogError($"image borders should have 1 child with a {nameof(Image)} component."); }

			int gridIndex = i;
			button.onClick.AddListener(() => SelectImage(gridIndex));
			images.Add(image);
		}

		uiImages = images.ToArray();
	}

	private void SetNavigation(int displayedImages)
	{
		Navigation navigation;

		if (displayedImages == gridSize)
		{
			Selectable bottomRight = uiImages[gridSize - 1].GetComponentInParent<Selectable>();
			navigation = bottomRight.navigation;
			navigation.selectOnRight = nextPage;
			bottomRight.navigation = navigation;

			navigation = nextPage.navigation;
			navigation.selectOnLeft = bottomRight;
			nextPage.navigation = navigation;
			//--------------------------------------------------------------------------------------------
			Selectable topRight = uiImages[imagesPerRow - 1].GetComponentInParent<Selectable>();
			navigation = topRight.navigation;
			navigation.selectOnRight = previousPage;
			topRight.navigation = navigation;

			navigation = previousPage.navigation;
			navigation.selectOnLeft = topRight;
			previousPage.navigation = navigation;
			return;
		}

		Selectable last = uiImages[displayedImages - 1].GetComponentInParent<Selectable>();
		navigation = last.navigation;
		navigation.selectOnRight = nextPage;
		last.navigation = navigation;

		navigation = nextPage.navigation;
		navigation.selectOnLeft = last;
		nextPage.navigation = navigation;

		if (displayedImages < imagesPerRow)
		{
			navigation = previousPage.navigation;
			navigation.selectOnLeft = last;
			previousPage.navigation = navigation;
			return;
		}

		Selectable topLast = uiImages[imagesPerRow - 1].GetComponentInParent<Selectable>();
		navigation = topLast.navigation;
		navigation.selectOnRight = previousPage;
		topLast.navigation = navigation;

		navigation = previousPage.navigation;
		navigation.selectOnLeft = topLast;
		previousPage.navigation = navigation;
	}

	private void SelectImage(int gridIndex)
	{
		int listIndex = gridSize * scrollIndex + gridIndex;
		string fileName = fileNames[listIndex];
		imageSelectedEvent?.Invoke(fileName);
	}

	private string[] GetFileNames()
	{
		List<string> temp = new();
		
		foreach (string fileFormat in acceptedFileFormats)
		{
			foreach (string fileName in Directory.GetFiles(imageDirectoryPath, $"*.{fileFormat}", SearchOption.AllDirectories))
			{
				string trimmedFileName = fileName.Substring(imageDirectoryPath.Length, fileName.Length - imageDirectoryPath.Length);
				temp.Add(trimmedFileName);
			}
		}
		
		return temp.ToArray();
	}
}