using System;
using System.IO;
using saxion_provided;
using UnityEngine;
using UnityEngine.UI;

public class ChosenImagesCommunicator : Singleton<ChosenImagesCommunicator>
{
	private const string FILE_NAME = "chosen_images.txt";

	[SerializeField] private Client networkingClient;
	[SerializeField] private Transform previewImageParent;
	private DataBaseSelector dataBaseSelector;

	private Image[] previewImages;
	private string filePath;

	public override void Awake()
	{
		base.Awake();

		if (networkingClient == null) { throw new UnassignedReferenceException("networking client is not set in the editor!"); }

		dataBaseSelector = this.GetComponentThrow<DataBaseSelector>();
		filePath = string.Concat(Application.persistentDataPath, Path.DirectorySeparatorChar, FILE_NAME);
		previewImages = new Image[previewImageParent.childCount];
		for (int i = 0; i < previewImageParent.childCount; ++i)
		{
			Image image = previewImageParent.GetChild(i).GetComponentThrow<Image>();
			previewImages[i] = image;
		}

		gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		if (!Player.Instance.dead) { return; }

		Packet packet = new();
		packet.Write(new RequestFileNames());
		networkingClient.SendData(packet);
		Debug.Log("Requesting fileNames");
	}

	public void RewriteDataBaseCache(GetFileNames serverObject)
	{
		if (!File.Exists(filePath)) { File.Create(filePath); }

		File.WriteAllLines(filePath, serverObject.fileNames);

		if (!gameObject.activeInHierarchy) { return; }

		try { DisplayPreviewImages(); }
		catch (FileNotFoundException e) { Debug.LogError(string.Concat(e.Message, Environment.NewLine, Environment.NewLine, e.StackTrace)); }
	}

	private void DisplayPreviewImages()
	{
		if (!File.Exists(filePath)) { throw new FileNotFoundException($"could not load file {filePath}"); }

		string[] fileNames = File.ReadAllLines(filePath);

		for (int i = 0; i < previewImages.Length; ++i)
		{
			Image previewImage = previewImages[i];
			if (i >= fileNames.Length)
			{
				previewImage.gameObject.SetActive(false);
				continue;
			}
			
			previewImage.gameObject.SetActive(true);

			string fileName = fileNames[i];
			if (dataBaseSelector.spriteCache.TryGetValue(fileName, out Sprite sprite))
			{
				previewImage.sprite = sprite;
				continue;
			}

			sprite = Utils.loadSpriteFromDisk(dataBaseSelector.imageDirectoryPath + fileName);
			previewImage.sprite = sprite;
		}
	}
}