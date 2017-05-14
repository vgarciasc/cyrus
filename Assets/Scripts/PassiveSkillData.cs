using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Passive Skill Data")]
public class PassiveSkillData : ScriptableObject {
	public enum TriggerOperation {AND, OR};

	public string name = "";

	public List<Trigger> triggers = new List<Trigger>();
	public TriggerOperation operation;

	public List<Effect> effects = new List<Effect>();
}
