using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerKind {
	ON_SUCCESSFUL_ATTACK, //triggered when character with skill receives/makes a successful attack
	ON_PHYSICAL_ATTACK, //triggered when character with skill makes a physical attack
	ON_MAGICAL_ATTACK, //triggered when character with skill makes a magical attack
	ON_CRIT_ATTACK, //triggered when character with skills receives/makes critical hit
	ON_MAKE_ATTACK, //triggered when character with skills makes an attack
	ON_RECEIVE_ATTACK, //triggered when character with skills receives an attack
	ON_SIMPLE_ATTACK, //triggered when character with skills receives/makes a simple attack
	ON_SIMPLE_COUNTER, //triggered when character receives/makes a counter attack
	ON_OWN_TURN_START, //triggered when the character's turn starts
	ON_START_MATCH, //triggered when match starts
	ON_MISS_ATTACK, //triggered when character with skill misses/dodges an attack
	ON_ADJACENT_ALLY_ATTACKED, //triggered when adjacent ally is attacked
	ON_ANY_ALLY_ATTACKED, //triggered when any ally is attacked
	ON_BLOCK, //triggered when character with skill defends/blocks an ally
	ON_BEING_DEFENDED_WITH_BLOCK //triggered when character with skill is defended with the block
};

[System.Serializable]
public class Trigger : System.Object {
	public TriggerKind kind = new TriggerKind();
}