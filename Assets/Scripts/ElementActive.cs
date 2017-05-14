using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectActive {
	SWAP, //swaps TARGET 1 with TARGET 2
	COMPLETE_ATTACK, //makes TARGET 1 complete-attack TARGET 2
	SIMPLE_ATTACK, //makes TARGET 1 simple-attack TARGET 2
	SIMPLE_COUNTER, //makes TARGET 1 simple-counter TARGET 2
	SHOW_LABEL, //shows label on TARGET 1
	BUFF_DEBUFF //casts buff/debuff on TARGET 1
};

public enum ElementActiveKind {TARGET, EFFECT};
public enum TargetActive {
	ADJACENT_SWAPPABLE, //ADJACENT (TARGET 1): targets allies adjacent to TARGET 1 that are swappable
	ENEMIES_LANE_SWAPPABLE, //ENEMIES_LANE (TARGET 1): targets enemies in lane of TARGET 1
	ENEMIES_LANE_ANY, //ENEMIES_LANE (TARGET 1): targets enemies in lane of TARGET 1 that are swappable
	ADJACENT_ANY //ADJACENT (TARGET 1): targets any allies adjacent to TARGET 1
};

[System.Serializable]
public class ElementActive : System.Object {
	//MAIN
	public ElementActiveKind kind = ElementActiveKind.TARGET;
	public List<int> targetIndex = new List<int>(); //-1 for SELF

	//TARGET
	public TargetActive target;

	//EFFECT
	public EffectActive effect;

	//MAIN
	[RangeAttribute(0f, 5f)]
	public float totalAttackMultiplier = 1f;
	public Buff buff;
	public bool showBuffLabel = true;
}
