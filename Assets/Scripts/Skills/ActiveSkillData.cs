using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Active Skill Data")]
public class ActiveSkillData : SkillData {
	public int canBeUsedEveryXTurns = 1;
	public List<ElementActive> elements = new List<ElementActive>();

	[HideInInspector]
	public int turnsSinceLastCast = -1;
}
