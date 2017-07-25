using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TeamManager))]
public class TeamManagerEditor : Editor {

	public override void OnInspectorGUI() {

		base.DrawDefaultInspector();
		TeamManager team = (TeamManager) target;

		if (GUILayout.Button("Add Character To Database")) {
			Undo.RecordObject(team, "Added Character");
			team.Add_Character(team.characterData);
		}
	}
}
