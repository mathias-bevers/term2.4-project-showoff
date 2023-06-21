using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using saxion_provided;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreManager : Singleton<HighScoreManager>
{
	public delegate List<(string, int)> RequestHighScoreDelegate();

	[SerializeField, Scene] private int mainMenuScene;

	public List<HighScoreData> highScoreDatas { get; private set; }
	public RequestHighScoreDelegate requestHighScoreDelegate { get; set; }

	private MapWalker mapWalker;
	private string highScoreFilePath;

	public override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;

		highScoreFilePath = string.Concat(Application.persistentDataPath, Path.DirectorySeparatorChar, "high_scores.txt");
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != mainMenuScene) { return; }

		List<(string, int)> fileEntries = ReadScoresFromFile().ToList();
		fileEntries.Sort((a, b) => b.Item2.CompareTo(a.Item2));
		highScoreDatas = fileEntries.Select((entry, i) => new HighScoreData(i + 1, entry.Item1, entry.Item2)).ToList();

		SetScores();
	}

	public void RewriteScoresToFile(List<(string, int)> scoreCollection)
	{
		if (scoreCollection.IsNullOrEmpty()) { return; }

		if (!File.Exists(highScoreFilePath)) { File.Create(highScoreFilePath); }

		List<string> linesToWrite = scoreCollection.Select(score => string.Concat(score.Item1, ',', score.Item2)).ToList();
		
		File.WriteAllLines(highScoreFilePath, linesToWrite);
		Debug.Log($"Written {string.Join(", ", scoreCollection)} to file.");
	}

	public void SendHighScoreToServer(string playerName, int score)
	{
		if (string.IsNullOrEmpty(playerName)) { throw new ArgumentNullException(nameof(playerName), "Cannot be null or empty"); }

		Packet packet = new();
		packet.Write(new AddHighScore(playerName, score));
		// Debug.Log($"Sending score to server: {name}, {score}");
		Player.Instance.client.SendData(packet);
	}

	private IEnumerable<(string, int)> ReadScoresFromFile()
	{
		if (!File.Exists(highScoreFilePath)) { return null; }

		List<(string, int)> fileEntries = new();
		foreach (string entry in File.ReadLines(highScoreFilePath))
		{
			if (string.IsNullOrEmpty(entry)) { continue; }

			try
			{
				string[] splitEntry = entry.Split(',');

				string playerName = splitEntry[0].Trim();
				string distanceInString = splitEntry[1].Trim();

				if (!int.TryParse(distanceInString, out int distanceRan)) { Debug.LogError($"could not parse \'{distanceInString}\' to an int!"); }

				fileEntries.Add((playerName, distanceRan));
			}
			catch (IndexOutOfRangeException) { Debug.LogWarning($"Could not process line: \'{entry}\'"); }
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
		AddHighScores server = new(localScores);

		Packet packet = new();
		packet.Write(server);
		return packet;
	}
}