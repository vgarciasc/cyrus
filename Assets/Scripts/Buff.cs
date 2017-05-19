using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType {
	ATT_FOR,
	ATT_DEF,
	ATT_INT,
	ATT_RES,
	ATT_AGI,
	ATT_DES,
	ATT_VIT,
	CRIT_MULTIPLIER, //multiplies probability of critical hits
	SWAP_TARGETTING, //changes what allies the user can swap with
	IGNORE_CHANCE, //adds to probability of blocking entire attack
	SWAP_BLOCKING, //changes if the user can be swapped with
	BARRIER, //character has a barrier
	DELAYED_ATTACK, //character will be attacked on buff end
	SWAP_ACTIONS_REFRESH, //character has more swap actions
	HEAL_OVER_TIME //character will heal a little every turn
};

public enum BuffGender {BUFF, DEBUFF, NONE};

[System.Serializable]
public class Buff : System.Object {
	public BuffType kind;
	public BuffGender gender;
	
	[HideInInspector]
	public string skillName = "";
	public int amount = 0;
	[RangeAttribute(0f, 2f)]
	public float multiplier = 1f;
	public int turnsDuration = 0; //-1 for infinite
	[HideInInspector]
	public int turnsLeft = 0;
	public bool stackable = true;
	[RangeAttribute(0f, 1f)]
	public float probability = 1f;

	//swap targetting
	public AllyTarget swapTarget = AllyTarget.ADJACENT;

	public override string ToString() {
		return kind.ToString() + ": " + amount;
	}

	public Buff (Buff b) {
		this.kind = b.kind;
		this.amount = b.amount;
		this.turnsLeft = b.turnsDuration; //starts with maxturns
		this.turnsDuration = b.turnsDuration;
		this.stackable = b.stackable;
		this.swapTarget = b.swapTarget;
		this.skillName = b.skillName;
		this.multiplier = b.multiplier;
		this.probability = b.probability;
	}

	public bool Equals(Buff b) {
		return (amount == b.amount &&
			multiplier == b.multiplier &&
			turnsDuration == b.turnsDuration &&
			stackable == b.stackable &&
			kind == b.kind);
	}
}
