using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PassiveSkillData))]
public class PassiveSkillEditor : Editor {

	PassiveSkillData skill;

	public override void OnInspectorGUI() {
		skill = (PassiveSkillData) target;
		GUI.skin.label.wordWrap = true;

		Draw_Title();
		EditorGUILayout.Space();
		Draw_Triggers();
		EditorGUILayout.Space();
		Draw_Effects();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// DrawDefaultInspector();
	}

	void Draw_Title() {
		EditorGUILayout.LabelField("Skill Name", EditorStyles.boldLabel);
		skill.title = EditorGUILayout.TextArea(skill.title);
	}

	void Draw_Triggers() {
		EditorGUILayout.LabelField("Triggers", EditorStyles.boldLabel);

		for (int i = 0; i < skill.triggers.Count; i++) {
			Trigger tri = skill.triggers[i];

			GUILayout.BeginHorizontal("Enum", EditorStyles.miniButton);	
			tri.kind = (TriggerKind) EditorGUILayout.EnumPopup(tri.kind);

			if (GUILayout.Button("X", EditorStyles.miniButtonRight)) {
				skill.triggers.RemoveAt(i);
				Undo.RecordObject(skill, "Remove Trigger");
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal("Box");	
		if (GUILayout.Button("Add")) {
			skill.triggers.Add(new Trigger());
			Undo.RecordObject(skill, "Add Trigger");
		}

		if (GUILayout.Button("Clear")) {
			skill.triggers.Clear();
			Undo.RecordObject(skill, "Clear Triggers");
		}

		skill.operation = (PassiveSkillData.TriggerOperation)
			EditorGUILayout.EnumPopup(skill.operation, EditorStyles.miniButton);
		
		GUILayout.EndHorizontal();
	}

	void Draw_Effects() {
		EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

		for (int i = 0; i < skill.effects.Count; i++) {
			GUILayout.BeginVertical("Box");
			GUILayout.BeginHorizontal();
			GUILayout.Label("Effect #" + (i+1));
			skill.effects[i].kind = (EffectPassive) EditorGUILayout.EnumPopup(skill.effects[i].kind);

			if (GUILayout.Button("X", EditorStyles.miniButtonRight)) {
				skill.effects.RemoveAt(i);
				Undo.RecordObject(skill, "Remove Effect");
			}
			GUILayout.EndHorizontal();
			
			switch (skill.effects[i].kind) {
				case EffectPassive.BUFF_DEBUFF:
					Draw_Buff(ref skill.effects[i].buff);
					break;
				case EffectPassive.PROTECT_ALLY:
					skill.effects[i].protectAlly.probability = 
						EditorGUILayout.Slider("Probability: " + 
							(skill.effects[i].protectAlly.probability * 100) + "%",
							skill.effects[i].protectAlly.probability,
							0f,
							1f);
					break;
			}
			
			GUILayout.EndVertical();
		}

		GUILayout.BeginHorizontal("Box");	
		if (GUILayout.Button("Add")) {
			skill.effects.Add(new Effect());
			Undo.RecordObject(skill, "Add Effect");
		}

		if (GUILayout.Button("Clear")) {
			skill.triggers.Clear();
			Undo.RecordObject(skill, "Clear Effects");
		}
		
		GUILayout.EndHorizontal();
	}

	void Draw_Buff(ref Buff buff) {
		buff.kind = (BuffType) EditorGUILayout.EnumPopup(buff.kind);
		
		string s_aux;

		switch (buff.kind) {
			case BuffType.ATT_AGI:
			case BuffType.ATT_FOR:
			case BuffType.ATT_DES:
			case BuffType.ATT_RES:
			case BuffType.ATT_DEF:
			case BuffType.ATT_INT:
				GUILayout.BeginVertical();
				
				buff.amount = EditorGUILayout.DelayedIntField("Amount:",
					buff.amount,
					EditorStyles.miniTextField);
				
				s_aux = buff.turnsDuration == -1 ? " (INFINITE!):" : ":";
				buff.turnsDuration = EditorGUILayout.DelayedIntField("Turns" + s_aux,
					buff.turnsDuration,
					EditorStyles.miniTextField);

				buff.stackable = EditorGUILayout.Toggle("Stackable:",
					buff.stackable);
				
				GUILayout.EndVertical();
				break;
			case BuffType.CRIT_MULTIPLIER:
				GUILayout.BeginVertical();

				buff.multiplier = 
					EditorGUILayout.Slider("Multiplier (x" +
						buff.multiplier + "): ",
						buff.multiplier,
						0f,
						5f);
				
				s_aux = buff.turnsDuration == -1 ? " (INFINITE!):" : ":";
				buff.turnsDuration = EditorGUILayout.DelayedIntField("Turns" + s_aux,
					buff.turnsDuration,
					EditorStyles.miniTextField);

				GUILayout.EndVertical();
				break;
		}
	}
}