using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillID {NULL, DESARMAR};
public enum SkillTargeting {NULL, ON_ATTACK, TARGET_ENEMY, TARGET_ALLY};

[CreateAssetMenu(menuName = "Custom/SkillData")]
public class SkillData : ScriptableObject {
	public string title = "";
	public SkillID ID = SkillID.NULL;
	public SkillTargeting targetingStyle = SkillTargeting.NULL;
	public List<Buff> buffs = new List<Buff>();
	public bool hasButton = true;

	[HeaderAttribute("ON ATTACK Triggering Skills")]
	[TooltipAttribute("If this skill is activated 'ON ATTACK', does it activate when the attack misses?")]
	public bool activateEvenWhenAttackMisses = false;
	[TooltipAttribute("If this skill is activated 'ON ATTACK', does it activate when the attack is a counter attack?")]
	public bool activateEvenWhenCounterAttack = false;
}
