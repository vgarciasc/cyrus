using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterSprite))]
public class CharacterSpriteEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		CharacterSprite cs = (CharacterSprite) target;
		GUI.skin.label.wordWrap = true;
		GUILayout.Label("These palettes don't change anything. They are just for a quick visualization of what the generator can generate.");
		if (GUILayout.Button("Generate New Palette")) {
			Undo.RecordObject(cs, "Generate New Palette");
			cs.Set_Random_Palette();
			// EditorUtility.SetDirty(cs);
		}
	}
}