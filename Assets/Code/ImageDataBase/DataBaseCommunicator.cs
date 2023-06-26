using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using saxion_provided;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataBaseCommunicator : Singleton<DataBaseCommunicator>
{
	private const string FILE_NAME = "chosen_images.txt";

	[SerializeField, Scene] private int mainMenuScene;
	[SerializeField] private Client networkingClient;
	[SerializeField] private Transform previewImageParent;
	[SerializeField] private Sprite oneXone;
	[SerializeField] private Button confirmSelectionButton;
	[SerializeField] private DataBaseSelector dataBaseSelector;

	private bool hasSelectedImage;
	private Image[] previewImages;
	private string filePath;
	private string selectedFileName;

	public override void Awake()
	{
		base.Awake();

		if (ReferenceEquals(networkingClient, null)) { throw new UnassignedReferenceException($"{nameof(networkingClient)} is not set in the editor!"); }

		if (ReferenceEquals(previewImageParent, null)) { throw new UnassignedReferenceException($"{nameof(previewImageParent)} is not set in the editor!"); }

		if (ReferenceEquals(oneXone, null)) { throw new UnassignedReferenceException($"{nameof(oneXone)} is not set in the editor!"); }

		if (ReferenceEquals(confirmSelectionButton, null)) { throw new UnassignedReferenceException($"{nameof(confirmSelectionButton)} is not set in the editor!"); }

		if (ReferenceEquals(dataBaseSelector, null)) { throw new UnassignedReferenceException($"{nameof(dataBaseSelector)} is not set in the editor!"); }

		filePath = string.Concat(Application.persistentDataPath, Path.DirectorySeparatorChar, FILE_NAME);
		previewImages = new Image[previewImageParent.childCount];

		dataBaseSelector.imageSelectedEvent += OnImageSelected;
		confirmSelectionButton.onClick.AddListener(() => WriteSelectionToServer(selectedFileName));

		for (int i = 0; i < previewImageParent.childCount; ++i)
		{
			Image image = previewImageParent.GetChild(i).GetComponentThrow<Image>();
			previewImages[i] = image;
		}

	}

	private void OnEnable()
	{
		if (!Player.Instance.dead) { return; }

		Packet packet = new();
		string[] readFileNames = ReadFileNames();
		AddFileNames fileNames = new(readFileNames);
		packet.Write(fileNames);
		networkingClient.SendData(packet);
		Debug.Log("Send local data to server");
	}

	private void OnImageSelected(string fileName)
	{
		Image previewImage = previewImages[0];
		selectedFileName = fileName;

		if (dataBaseSelector.spriteCache.TryGetValue(fileName, out Sprite sprite))
		{
			previewImage.sprite = sprite;
			return;
		}

		sprite = Utils.LoadSpriteFromDisk(dataBaseSelector.imageDirectoryPath + fileName);
		previewImage.sprite = sprite;
	}

	public void RewriteDataBaseCache(GetFileNames serverObject)
	{
		if (!File.Exists(filePath)) { File.Create(filePath).Close(); }

		try { File.WriteAllLines(filePath, serverObject.fileNames); }
		catch (IOException e) { Debug.LogError(string.Concat($"Could not write to file \'{filePath}\', it is probably used by another process!", Environment.NewLine, Environment.NewLine, e)); }

		if (hasSelectedImage)
		{
			SceneManager.LoadScene(0);
			return;
		}

		if (!gameObject.activeInHierarchy) { return; }

		try { DisplayPreviewImages(); }
		catch (FileNotFoundException e) { Debug.LogError(string.Concat(e.Message, Environment.NewLine, Environment.NewLine, e.StackTrace)); }
		catch (IOException e) { Debug.LogError(string.Concat($"Could not write to file \'{filePath}\', it is probably used by another process!", Environment.NewLine, Environment.NewLine, e)); }
	}

	private void DisplayPreviewImages()
	{
		string[] fileNames = ReadFileNames();
		Array.Reverse(fileNames);

		previewImages[0].sprite = oneXone;

		for (int i = 0; i < previewImages.Length - 1; ++i)
		{
			Image previewImage = previewImages[i + 1];
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

			sprite = Utils.LoadSpriteFromDisk(dataBaseSelector.imageDirectoryPath + fileName);
			previewImage.sprite = sprite;
		}
	}

	private string[] ReadFileNames()
	{
		if (!File.Exists(filePath)) { return Array.Empty<string>(); }


		try
		{
			List<string> temp = new();
			string[] readLines = File.ReadAllLines(filePath);
			if (readLines.Length <= 0) { return Array.Empty<string>(); }

			int start = 0;
			if (readLines.Length > Settings.MAX_MEMES) { start = readLines.Length - Settings.MAX_MEMES; }

			for (int i = start; i < readLines.Length; ++i) { temp.Add(readLines[i]); }

			return temp.ToArray();
		}
		catch (IOException e) { Debug.LogError(string.Concat($"Could not write to file \'{filePath}\', it is probably used by another process!", Environment.NewLine, Environment.NewLine, e)); }

		return Array.Empty<string>();
	}

	private void WriteSelectionToServer(string fileName)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			Debug.LogWarning("No selection has been made");
			return;
		}

		Packet packet = new();
		AddFileName addFileName = new(fileName);
		packet.Write(addFileName);
		networkingClient.SendData(packet);

		hasSelectedImage = true;
	}
}