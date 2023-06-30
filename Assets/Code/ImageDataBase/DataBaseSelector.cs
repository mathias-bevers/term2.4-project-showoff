using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DataBaseSelector : MonoBehaviour
{
	public event Action<string> imageSelectedEvent;
	private const string DIRECTORY_NAME = "BillboardImages";
	private const string CHOSEN_IMAGES_FILE_NAME = "chosen_images.txt";

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
	[SerializeField] private Button selectButton;

	public Dictionary<string, Sprite> spriteCache { get; private set; }
	public string imageDirectoryPath { get; private set; }


	private DataBaseDisplayElement[] displayElements;
	private Dictionary<int, int> pagesStart;
	private int scrollIndex;
	private int gridSize;
	private int pages;
	private Navigation originalPreviousNavigation;
	private Navigation originalNextNavigation;
	private List<string> displayingFileNames;
	private string[] chosenImages;
	private string[] fileNames;

	private void Awake()
	{
		spriteCache = new Dictionary<string, Sprite>();
		gridSize = imagesPerRow * rows;
		imageDirectoryPath = string.Concat(Application.streamingAssetsPath, Path.DirectorySeparatorChar, DIRECTORY_NAME, Path.DirectorySeparatorChar);
		fileNames = GetFileNames();

		string path = string.Concat(Application.persistentDataPath, Path.DirectorySeparatorChar, CHOSEN_IMAGES_FILE_NAME);

		try
		{
			chosenImages = File.ReadAllLines(path).Where(s => !string.IsNullOrEmpty(s)).ToArray();
		}
		catch (FileNotFoundException)
		{
			chosenImages = Array.Empty<string>();
			Debug.LogWarning("Could not find file, creating...");
			File.Create(path).Close();
		}

		pages = (int)Math.Ceiling((float)(fileNames.Length - chosenImages.Length) / gridSize);
		pagesStart = new Dictionary<int, int>();
		displayingFileNames = new List<string>(gridSize);
		originalPreviousNavigation = previousPage.navigation;
		originalNextNavigation = nextPage.navigation;

		nextPage.onClick.AddListener(() => Scroll(1));
		previousPage.onClick.AddListener(() => Scroll(-1));
	}

	private void OnEnable()
	{
		SetupGrid();
		DisplayImages();

		EventSystem.current.SetSelectedGameObject(displayElements[0].gameObject);
	}

	private List<Sprite> LoadSprites()
	{
		int start = gridSize * scrollIndex;
		if (pagesStart.TryGetValue(scrollIndex - 1, out int _start)) { start = _start; }
		
		List<Sprite> sprites = new(imagesPerRow * rows);
		displayingFileNames.Clear();

		int i = start;
		while (sprites.Count < gridSize && i < fileNames.Length)
		{
			string fileName = fileNames[i];
			++i;

			if (chosenImages.Contains(fileName)) { continue; }

			displayingFileNames.Add(fileName);

			if (spriteCache.TryGetValue(fileName, out Sprite sprite))
			{
				sprites.Add(sprite);
				continue;
			}

			sprite = Utils.LoadSpriteFromDisk(imageDirectoryPath + fileName);
			sprites.Add(sprite);
			spriteCache.Add(fileName, sprite);
		}

		pagesStart.TryAdd(scrollIndex, i);
		return sprites;
	}

	private void DisplayImages()
	{
		List<Sprite> loaded = LoadSprites();
		for (int i = 0; i < displayElements.Length; ++i)
		{
			DataBaseDisplayElement displayElement = displayElements[i];
			if (i >= loaded.Count)
			{
				displayElement.gameObject.SetActive(false);
				continue;
			}

			displayElement.gameObject.SetActive(true);
			displayElement.SetImage(loaded[i]);
			displayElement.ResetNavigation();
		}

		if (loaded.Count == gridSize)
		{
			previousPage.navigation = originalPreviousNavigation;
			nextPage.navigation = originalNextNavigation;
			return;
		}

		SetNavigation(loaded.Count);
	}

	private void Scroll(int amount)
	{
		scrollIndex += amount;
		scrollIndex %= pages;

		if (scrollIndex < 0) { scrollIndex = pages - 1; }

		DisplayImages();
	}

	private void SetupGrid()
	{
		List<DataBaseDisplayElement> dataBaseDisplayElements = new();

		for (int i = 0; i < content.childCount; ++i)
		{
			DataBaseDisplayElement displayElement = content.GetChild(i).GetComponentThrow<DataBaseDisplayElement>();

			if (!displayElement.TryGetComponent(out Button button)) { Debug.Log($"All children in {content.name} need to have a {nameof(Button)} component"); }

			if (displayElement.imageToDisplay == null) { Debug.LogError("image borders should have set the image serialize field"); }

			int gridIndex = i;
			button.onClick.AddListener(() => SelectImage(gridIndex));
			dataBaseDisplayElements.Add(displayElement);
		}

		displayElements = dataBaseDisplayElements.ToArray();
	}

	private void SetNavigation(int displayedImages)
	{
		Navigation navigation;

		int start = Math.Min(displayedImages, imagesPerRow);
		for (int i = start; i >= 0; --i)
		{
			Selectable cur = displayElements[displayedImages - i].selectable;
			navigation = cur.navigation;
			navigation.selectOnDown = selectButton;
			cur.navigation = navigation;
		}
		
		Selectable last = displayElements[displayedImages - 1].selectable;
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

		Selectable topLast = displayElements[imagesPerRow - 1].selectable;
		navigation = topLast.navigation;
		navigation.selectOnRight = previousPage;
		topLast.navigation = navigation;

		navigation = previousPage.navigation;
		navigation.selectOnLeft = topLast;
		previousPage.navigation = navigation;
	}

	private void SelectImage(int gridIndex)
	{
		
		string fileName = displayingFileNames[gridIndex];
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