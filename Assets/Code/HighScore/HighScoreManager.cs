using System;
using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
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

		highScoreDatas = ReadScoresFromFile();
		SetScores();
	}

	private void WriteScoreToFile()
	{
		if (!File.Exists(highScoreFilePath)) { File.Create(highScoreFilePath); }

		string playerName = "test";
		int distanceRan = UnityEngine.Random.Range(1, 1000000001);

		string fileLine = string.Concat(playerName, ',', distanceRan, Environment.NewLine);
		File.AppendAllText(highScoreFilePath, fileLine);
	}

	private List<HighScoreData> ReadScoresFromFile()
	{
		if (!File.Exists(highScoreFilePath)) { return new List<HighScoreData>(); }

		List<(string, int)> fileEntries = new();
		foreach (string entry in File.ReadLines(highScoreFilePath))
		{
			string[] splitEntry = entry.Split(',');

			string playerName = splitEntry[0].Trim();
			string distanceInString = splitEntry[1].Trim();

			if (!int.TryParse(distanceInString, out int distanceRan)) { Debug.LogError($"could not parse \'{distanceInString}\' to an int!"); }

			fileEntries.Add((playerName, distanceRan));
		}

		fileEntries.Sort((a, b) => b.Item2.CompareTo(a.Item2));

		List<HighScoreData> existingEntries = new();
		for (int i = 0; i < fileEntries.Count; ++i)
		{
			(string, int) fileEntry = fileEntries[i];
			existingEntries.Add(new HighScoreData(i + 1, fileEntry.Item1, fileEntry.Item2));
		}

		return existingEntries;
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
}