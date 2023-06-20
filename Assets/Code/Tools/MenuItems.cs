using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MenuItems : MonoBehaviour
{
	[MenuItem("Window/MUDFISH/Open PD path")]
	private static void OpenPersistentDataPath()
	{
		string path = Application.persistentDataPath;
		UnityEngine.Debug.Log($"Opened to path: <i>{path}</i>");
		Process.Start(path);
	}

	[MenuItem("Window/MUDFISH/Clear player uploads")]
	private static void ClearPersistentDataPath()
	{
		string path = Application.persistentDataPath + "/player_uploads/";
		DirectoryInfo uploadsDirectory = new(path);

		foreach (FileInfo file in uploadsDirectory.GetFiles()) { file.Delete(); }

		foreach (DirectoryInfo directory in uploadsDirectory.GetDirectories()) { directory.Delete(true); }
	}
}