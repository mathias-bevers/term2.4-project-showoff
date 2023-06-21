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
}