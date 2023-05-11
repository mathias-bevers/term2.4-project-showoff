using UnityEditor;
using UnityEngine;

public class MenuItems : MonoBehaviour
{
	[MenuItem("Assets/Create/Showoff/Pickup Prefab", false, 1)]
	public static void CreatePickupPrefab(MenuCommand command)
	{
		string filePath = Selection.assetGUIDs.Length == 0 ? "Assets" : AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
		filePath += "/New Pickup prefab.prefab";

		GameObject go = new("New Pickup prefab")
		{
			hideFlags = HideFlags.HideInHierarchy,
		};
		go.AddComponent<Pickup>();
		go.AddComponent<MeshFilter>();
		go.AddComponent<MeshRenderer>();
		
		PrefabUtility.SaveAsPrefabAsset(go, filePath);

		DestroyImmediate(go); // So the object is deleted in the scene
	}
}