using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BillboardManager : Singleton<BillboardManager>
{
	private const string DEFAULT_MATERIAL_NAME = "Universal Render Pipeline/Unlit";
	private const string TXT_FILE_NAME = "chosen_images.txt";
	private const string IMAGE_DIRECTORY_NAME = "BillboardImages";
	private Dictionary<string, Material> materialCache;
	private List<string> activeImages;


	private Material defaultMaterial;
	private string txtFilePath;
	private string imageDirectoryPath;
	private string[] fileNames;

	public override void Awake()
	{
		txtFilePath = string.Concat(Application.persistentDataPath, Path.DirectorySeparatorChar, TXT_FILE_NAME);
		imageDirectoryPath = string.Concat(Application.streamingAssetsPath, Path.DirectorySeparatorChar, IMAGE_DIRECTORY_NAME, Path.DirectorySeparatorChar);
		fileNames = ReadFileNames(txtFilePath);
		activeImages = new List<string>();
		materialCache = new Dictionary<string, Material>(); 
		Shader shader = Shader.Find(DEFAULT_MATERIAL_NAME);
		defaultMaterial = new Material(shader);

		Player.Instance.deathEvent += materialCache.Clear;
	}

	public string RequestSetup(Billboard billboard)
	{
		billboard.destroyingEvent += OnBillboardDestroy;

		string imageToLoadName = activeImages.IsNullOrEmpty() || activeImages.Count < fileNames.Length ? fileNames.Except(activeImages).ToList().GetRandomElement() : fileNames.GetRandomElement();

		if (string.IsNullOrEmpty(imageToLoadName))
		{
			billboard.renderer.material = defaultMaterial;
			return null;
		}

		activeImages.Add(imageToLoadName);

		if (materialCache.TryGetValue(imageToLoadName, out Material material))
		{
			billboard.renderer.material = material;
			return imageToLoadName;
		}

		material = new Material(Shader.Find(DEFAULT_MATERIAL_NAME)) { mainTexture = Utils.LoadTextureFromDisk(imageDirectoryPath + imageToLoadName) };
		materialCache.Add(imageToLoadName, material);
		billboard.renderer.material = material;
		return imageToLoadName;
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
		if (!activeImages.Contains(billboard.displayingImageName)) { return; }

		activeImages.Remove(billboard.displayingImageName);
	}
}