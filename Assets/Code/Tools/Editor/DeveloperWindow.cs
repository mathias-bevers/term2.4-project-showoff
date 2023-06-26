using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Code.Tools.Editor
{
	public class DeveloperWindow : EditorWindow
	{
		private const string SCENE_DIRECTORY = "Assets/Scenes/";

		private float spacingY;
		private GameObject replacer;
		private int sceneSelectedIndex;
		private string suffix;
		private string[] sceneNames;

		private void OnEnable() { sceneNames = FetchSceneNames(); }

		private void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			LoadScene();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			ReloadScripts();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			ReplaceWith();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			AddSuffix();
			EditorGUILayout.EndHorizontal();
		}

		[MenuItem("Window/MUDFISH/Developer Window")]
		private static void ShowWindow()
		{
			DeveloperWindow window = GetWindow<DeveloperWindow>();
			window.titleContent = new GUIContent("Devs' window");
			window.Show();
		}

		private void LoadScene()
		{
			if (GUILayout.Button("Fetch Names")) { sceneNames = FetchSceneNames(); }

			sceneSelectedIndex = EditorGUILayout.Popup(sceneSelectedIndex, sceneNames);

			if (!GUILayout.Button("Load selected scene")) { return; }

			try
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorSceneManager.OpenScene(string.Concat(SCENE_DIRECTORY, sceneNames[sceneSelectedIndex], ".unity"));
			}
			catch (ArgumentException e) { Debug.LogError(string.Concat(e.Message)); }
		}

		private void ReloadScripts()
		{
			if (!GUILayout.Button("Reload Scripts")) { return; }

			EditorUtility.RequestScriptReload();
		}

		private void ReplaceWith()
		{
			replacer = EditorGUILayout.ObjectField(replacer, typeof(GameObject), false) as GameObject;

			if (!GUILayout.Button("Replace")) { return; }

			if (replacer == null) { Debug.LogError($"Make sure the {nameof(replacer)} field is assigned"); }

			foreach (GameObject gameObject in Selection.gameObjects)
			{
				Transform transform = gameObject.transform;
				Transform parent = transform.parent;

				Vector3 position = transform.position;
				Vector3 scale = transform.localScale;
				Quaternion rotation = transform.rotation;

				GameObject instantiatedReplacer = Instantiate(replacer, position, rotation, parent);
				instantiatedReplacer.transform.localScale = scale;
				instantiatedReplacer.name = replacer.name;

				DestroyImmediate(gameObject);
			}
		}

		private void AddSuffix()
		{
			suffix = EditorGUILayout.TextField(suffix);

			if (!GUILayout.Button("Add suffix")) { return; }

			if (string.IsNullOrEmpty(suffix)) { Debug.LogError("Make sure textfield is not empty"); }

			foreach (GameObject gameObject in Selection.gameObjects) { gameObject.name += $"_{suffix}"; }

			suffix = string.Empty;
		}

		private static string[] FetchSceneNames()
		{
			List<string> temp = new();

			foreach (string path in Directory.GetFiles("Assets/Scenes/", "*.unity", SearchOption.AllDirectories))
			{
				string sceneName = path[(path.LastIndexOf('/') + 1)..];
				sceneName = Path.GetFileNameWithoutExtension(sceneName);
				temp.Add(sceneName);
			}

			return temp.ToArray();
		}
	}
}