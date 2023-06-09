﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	[SerializeField] private Sprite noImageSelectedSprite;
	[SerializeField] private Button confirmSelectionButton;
	[SerializeField] private DataBaseSelector dataBaseSelector;
	private bool hasSelectedImage;
	private Image[] previewImages;

	private string imageDirectoryPath;
	private string txtFilePath;
	private string selectedFileName;

	public override void Awake()
	{
		base.Awake();

		// if (ReferenceEquals(networkingClient, null)) { throw new UnassignedReferenceException($"{nameof(networkingClient)} is not set in the editor!"); }

		if (ReferenceEquals(previewImageParent, null)) { throw new UnassignedReferenceException($"{nameof(previewImageParent)} is not set in the editor!"); }

		if (ReferenceEquals(noImageSelectedSprite, null)) { throw new UnassignedReferenceException($"{nameof(noImageSelectedSprite)} is not set in the editor!"); }

		if (ReferenceEquals(confirmSelectionButton, null)) { throw new UnassignedReferenceException($"{nameof(confirmSelectionButton)} is not set in the editor!"); }

		if (ReferenceEquals(dataBaseSelector, null)) { throw new UnassignedReferenceException($"{nameof(dataBaseSelector)} is not set in the editor!"); }

		imageDirectoryPath = string.Concat(Application.streamingAssetsPath, Path.DirectorySeparatorChar, "BillboardImages", Path.DirectorySeparatorChar);
		txtFilePath = string.Concat(Application.persistentDataPath, Path.DirectorySeparatorChar, FILE_NAME);
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
#if UNITY_EDITOR
		if (!Player.IsInitialized) { return; }
#endif

		if (!Player.Instance.dead) { return; }

		if (networkingClient == null)
		{
			DisplayPreviewImages();
			return;
		}

		Packet packet = new();
		string[] readFileNames = ReadFileNames();
		AddFileNames fileNames = new(readFileNames);
		packet.Write(fileNames);
		networkingClient.SendData(packet);
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
		if (!File.Exists(txtFilePath)) { File.Create(txtFilePath).Close(); }

		try { File.WriteAllLines(txtFilePath, serverObject.fileNames); }
		catch (IOException e) { Debug.LogError(string.Concat($"Could not write to file \'{txtFilePath}\', it is probably used by another process!", Environment.NewLine, Environment.NewLine, e)); }

		if (hasSelectedImage)
		{
			SceneManager.LoadScene(mainMenuScene);
			return;
		}

		if (!gameObject.activeInHierarchy) { return; }

		try { DisplayPreviewImages(); }
		catch (FileNotFoundException e) { Debug.LogError(string.Concat(e.Message, Environment.NewLine.Repeat(2), e.StackTrace)); }
		catch (IOException e) { Debug.LogError(string.Concat($"Could not write to file \'{txtFilePath}\', it is probably used by another process!", Environment.NewLine.Repeat(2), e)); }
	}

	[Button]
	private void DisplayPreviewImages()
	{
		string[] fileNames = ReadFileNames();
		Array.Reverse(fileNames);

		previewImages[0].sprite = noImageSelectedSprite;

		for (int i = 0; i < previewImages.Length - 1; ++i)
		{
			Image previewImage = previewImages[i + 1];
			GameObject previewImageGameObject = previewImage.gameObject;
			if (i >= fileNames.Length)
			{
				previewImageGameObject.SetActive(false);
				continue;
			}

			previewImageGameObject.SetActive(true);

			string fileName = fileNames[i];
			Sprite sprite;
			if (dataBaseSelector != null && dataBaseSelector.spriteCache != null)
			{
				if (dataBaseSelector.spriteCache.TryGetValue(fileName, out sprite))
				{
					previewImage.sprite = sprite;
					continue;
				}
			}
			else { Debug.Log($"dataBase: {dataBaseSelector}, cache: {dataBaseSelector?.spriteCache}"); }

			sprite = Utils.LoadSpriteFromDisk(imageDirectoryPath + fileName);
			if (sprite == null)
			{
				previewImageGameObject.SetActive(false);
				continue;
			}

			previewImage.sprite = sprite;
		}
	}

	private string[] ReadFileNames()
	{
		if (!File.Exists(txtFilePath)) { return Array.Empty<string>(); }

		try
		{
			List<string> temp = new();
			string[] readLines =File.ReadAllLines(txtFilePath).Where(line => !string.IsNullOrEmpty(line)).ToArray();
			int readLinesLength = readLines.Length;

			if (readLinesLength <= 0) { return Array.Empty<string>(); }

			int start = Math.Max(0, readLinesLength - Settings.MAX_MEMES);

			for (int i = start; i < readLinesLength; ++i) { temp.Add(readLines[i]); }

			return temp.ToArray();
		}
		catch (IOException e) { Debug.LogError(string.Concat($"Could not write to file \'{txtFilePath}\', it is probably used by another process!", Environment.NewLine, Environment.NewLine, e)); }

		return Array.Empty<string>();
	}

	private void WriteSelectionToServer(string fileName)
	{
		if (networkingClient == null)
		{
			File.AppendAllText(txtFilePath, '\n' + fileName);
			SceneManager.LoadScene(mainMenuScene);
			return;
		}

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