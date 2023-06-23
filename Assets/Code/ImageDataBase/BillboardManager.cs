using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BillboardManager : Singleton<BillboardManager>
{
	private const string TXT_FILE_NAME = "chosen_images.txt";
	private const string IMAGE_DIRECTORY_NAME = "BillboardImages";

	[SerializeField] private Sprite oneXone; 

	private Dictionary<string, Sprite> spriteCache;
	private List<string> activeImages;
	private string txtFilePath;
	private string imageDirectoryPath;
	private string[] fileNames;

	public override void Awake()
	{
		txtFilePath = string.Concat(Application.persistentDataPath, Path.DirectorySeparatorChar, TXT_FILE_NAME);
		imageDirectoryPath = string.Concat(Application.streamingAssetsPath, Path.DirectorySeparatorChar, IMAGE_DIRECTORY_NAME, Path.DirectorySeparatorChar);
		fileNames = ReadFileNames(txtFilePath);
		activeImages = new List<string>();
		spriteCache = new Dictionary<string, Sprite>();

		if (ReferenceEquals(oneXone, null)) { throw new UnassignedReferenceException($"{nameof(oneXone)} is not set in the editor!"); }
		
		Player.Instance.deathEvent += spriteCache.Clear; 
	}

	public string[] RequestSetup(Billboard billboard)
	{
		billboard.destroyingEvent += OnBillboardDestroy;

		foreach (Image image in billboard.images)
		{
			string imageToLoadName = activeImages.IsNullOrEmpty() || activeImages.Count < fileNames.Length ? fileNames.Except(activeImages).ToList().GetRandomElement() : fileNames.GetRandomElement();

			if (string.IsNullOrEmpty(imageToLoadName))
			{
				image.sprite = oneXone;
				continue;
			}
			
			activeImages.Add(imageToLoadName);

			if (spriteCache.TryGetValue(imageToLoadName, out Sprite sprite)) 
			{
				image.sprite = sprite;
				continue;
			}

			sprite = Utils.LoadSpriteFromDisk(imageDirectoryPath + imageToLoadName);
			image.sprite = sprite;
		}

		return activeImages.Skip(Math.Max(0, activeImages.Count - billboard.images.Length)).ToArray();
	}

	private string[] ReadFileNames(string path)
	{
		if (!File.Exists(path)) { return Array.Empty<string>(); }

		try { return File.ReadAllLines(txtFilePath).Where(line => !string.IsNullOrEmpty(line)).ToArray(); }
		catch (IOException e) { Debug.LogError(string.Concat($"Could not write to file \'{txtFilePath}\', it is probably used by another process!", Environment.NewLine, Environment.NewLine, e)); }

		return Array.Empty<string>();
	}

	private void OnBillboardDestroy(Billboard billboard)
	{
		foreach (string displayingImageName in billboard.displayingImageNames)
		{
			if (!activeImages.Contains(displayingImageName))
			{
				continue;
			}
			
			activeImages.Remove(displayingImageName);
		}
	}
}