using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ImageSearcher : MonoBehaviour
{
	[SerializeField] private int imagesPerRow;
	[SerializeField] private int visibleRows;

	private Dictionary<string, Sprite> spriteCache; 
	private string imageDirectoryPath;
	private string[] fileNames;
	private int scrollIndex = 0;

	// private Image[]

	private readonly string[] acceptedFileFormats =
	{
		"jpg",
		"jpeg",
		"png",
		"tiff",
		"bmp",
	};

	private void Awake()
	{
		imageDirectoryPath = string.Concat(Application.streamingAssetsPath, Path.DirectorySeparatorChar, "BillboardImages", Path.DirectorySeparatorChar);
		
		fileNames = acceptedFileFormats.SelectMany(fileFormat => Directory.GetFiles(imageDirectoryPath, $"*.{fileFormat}", SearchOption.AllDirectories)).ToArray();
	}

	private void LoadImages()
	{
		int start = imagesPerRow * scrollIndex;
		int max = (imagesPerRow * visibleRows) + start;
		int end = Math.Min(max, fileNames.Length);

		List<Sprite> sprites = new(imagesPerRow * visibleRows);
		for (int i = start; i < end; ++i)
		{
			string fileName = fileNames[i];	
			
			if (spriteCache.TryGetValue(fileName, out Sprite value))
			{
				sprites.Add(value);
				continue;
			}
			
			byte[] inBytes = File.ReadAllBytes(imageDirectoryPath + fileName);
			
		}
	}

	private void Scroll(int amount)
	{
		scrollIndex += amount;
		// scrollIndex %= 
	}
}