using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

		highScoreFilePath = string.Concat(Application.persistentDataPath, System.IO.Path.DirectorySeparatorChar, "high_scores.txt");
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != mainMenuScene) { return; }
		
		List<(string, int)> fileEntries = ReadScoresFromFile().ToList();
		fileEntries.Sort((a,b) => b.Item2.CompareTo(a.Item2));
		highScoreDatas = fileEntries.Select((entry, i) => new HighScoreData(i + 1, entry.Item1, entry.Item2)).ToList();

		SetScores();
	}

	public void RewriteScoresToFile(List<(string, int)> scoreCollection)
	{
		if (scoreCollection.IsNullOrEmpty()) { return; }

		if (!File.Exists(highScoreFilePath)) { File.Create(highScoreFilePath); }
		
		IEnumerable<string> linesToWrite = scoreCollection.Select(score => string.Concat(score.Item1, ',', score.Item2));

		File.WriteAllLines(highScoreFilePath, linesToWrite);
	}

	public void WriteScoreToFile(string playerName, int distanceRan)
	{
		if (string.IsNullOrEmpty(playerName)) { throw new ArgumentNullException(nameof(playerName), "The given name is cannot be empty"); }

		if (distanceRan <= 1) { throw new ArgumentOutOfRangeException(nameof(distanceRan), $"\'{distanceRan}\' is not a valid distance"); }

		if (!File.Exists(highScoreFilePath)) { File.Create(highScoreFilePath); }

		string fileLine = string.Concat(playerName, ',', distanceRan, Environment.NewLine);
		File.AppendAllText(highScoreFilePath, fileLine);
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
		HighScoresList serverList = new(localScores);

		Packet packet = new();
		packet.Write(serverList);
		return packet;
	}
}