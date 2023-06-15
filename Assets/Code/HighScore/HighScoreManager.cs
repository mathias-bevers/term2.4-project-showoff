using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using saxion_provided;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreManager : Singleton<HighScoreManager>
{
	[SerializeField, Scene] private int mainMenuScene;

	public List<HighScoreData> highScoreDatas { get; private set; }
	private MapWalker mapWalker;

	private string highScoreFilePath;

	public override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;

		highScoreFilePath = string.Concat(Application.persistentDataPath, System.IO.Path.DirectorySeparatorChar, "high_scores.txt");
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != mainMenuScene) { return; }

		highScoreDatas = GetAllHighScores();
		SetScores();
	}

	public void WriteScoreToFile(string playerName, int distanceRan)
	{
		if (string.IsNullOrEmpty(playerName)) { throw new ArgumentNullException(nameof(playerName), "The given name is cannot be empty"); }

		if (distanceRan <= 1) { throw new ArgumentOutOfRangeException(nameof(distanceRan), $"\'{distanceRan}\' is not a valid distance"); }

		if (!File.Exists(highScoreFilePath)) { File.Create(highScoreFilePath); }

		string fileLine = string.Concat(playerName, ',', distanceRan, Environment.NewLine);
		File.AppendAllText(highScoreFilePath, fileLine);
	}

	private List<HighScoreData> GetAllHighScores()
	{
		List<HighScoreData> orderedScores = new List<HighScoreData>();
		return orderedScores;
	}

	private IEnumerable<(string, int)> ReadScoresFromFile()
	{
		if (!File.Exists(highScoreFilePath)) { return null; }

		List<(string, int)> fileEntries = new();
		foreach (string entry in File.ReadLines(highScoreFilePath))
		{
			string[] splitEntry = entry.Split(',');

			string playerName = splitEntry[0].Trim();
			string distanceInString = splitEntry[1].Trim();

			if (!int.TryParse(distanceInString, out int distanceRan)) { Debug.LogError($"could not parse \'{distanceInString}\' to an int!"); }

			fileEntries.Add((playerName, distanceRan));
		}

		return fileEntries;
	}

	private void SetScores()
	{
		Transform hsBoard = FindObjectOfType<HighScorePanel>().transform.parent;

		for (int i = 0; i < hsBoard.childCount; ++i)
		{
			HighScorePanel panel = hsBoard.GetChild(i).GetComponent<HighScorePanel>();
			panel.SetScore(i >= highScoreDatas.Count ? null : highScoreDatas[i]);
		}
	}

	public Packet LocalScoresAsPacket()
	{
		IEnumerable<(string, int)> localScores = ReadScoresFromFile();
		HighScoresList serverList = new(localScores);
		
		Packet packet = new();
		packet.Write(serverList);
		return packet;
	}
}