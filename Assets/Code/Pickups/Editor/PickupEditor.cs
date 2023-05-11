using System;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Pickup))]
public class PickupEditor : Editor
{
	private SerializedProperty pickupData;

	private void OnEnable()
	{
		pickupData = serializedObject.FindProperty("<data>k__BackingField");
		StringBuilder sb = new StringBuilder("Pickup properties:\n");
		var it = serializedObject.GetIterator();
		while (it.Next(true))
		{
			sb.AppendLine(it.name);
		}
		
		Debug.Log(sb.ToString());
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		if (pickupData == null)
		{
			EditorGUILayout.HelpBox("Please set a data preset", MessageType.Error);
			return;
		}
		
		EditorGUILayout.LabelField("WIP");
	}
}