using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Passive Skill Data")]
public class PassiveSkillData : SkillData {
	public enum TriggerOperation {AND, OR};

	public List<Trigger> triggers = new List<Trigger>();
	public TriggerOperation operation;

	public List<Effect> effects = new List<Effect>();
}

public class SkillData : ScriptableObject {
	public string title = "DEFAULT";
	public string description = "default";
}