using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetKind {
	LAST_DEFENDER, //target is last character to receive an attack
	LAST_ATTACKER, //target is last character to attack
	ADJACENT_TO_CASTER, //target is all characters adjacent to skill caster
	CASTER //target is the skill caster
};

[CreateAssetMenu(menuName = "Custom/Passive Skill Data")]
public class PassiveSkillData : ScriptableObject {
	public string name = "";

	public List<Trigger> triggers = new List<Trigger>();
	public List<Effect> effects = new List<Effect>();
}
