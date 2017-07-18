using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor {

	public override void OnInspectorGUI() {

		base.DrawDefaultInspector();
		DungeonGenerator gen = (DungeonGenerator) target;
		
		// if (GUILayout.Button("Populate Tiles")) {
		// 	gen.Populate_Tiles();
		// }

		if (GUILayout.Button("Generate Dungeon")) {
			Undo.RecordObject(gen, "Generate Dungeon");
			gen.Generate_Dungeon();
			Canvas.ForceUpdateCanvases();
		}
	}
}
