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
}
