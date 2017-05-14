using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerKind {
	ON_SUCCESSFUL_ATTACK, //triggered when player with skill receives/makes a successful attack
	ON_PHYSICAL_ATTACK, //triggered when player with skill makes a physical attack
	ON_MAGICAL_ATTACK, //triggered when player with skill makes a magical attack
	ON_CRIT_ATTACK, //triggered when player with skills receives/makes critical hit
	ON_MAKE_ATTACK, //triggered when player with skills makes an attack
	ON_RECEIVE_ATTACK, //triggered when player with skills receives an attack
	ON_SIMPLE_ATTACK, //triggered when player with skills receives/makes a simple attack
	ON_SIMPLE_COUNTER, //triggered when player receives/makes a counter attack
	ON_OWN_TURN_START, //triggered when the character's turn starts
	ON_START_MATCH, //triggered when match starts
	ON_MISS_ATTACK //triggered when player with skill misses/dodges an attack
};

[System.Serializable]
public class Trigger : System.Object {
	public TriggerKind kind = new TriggerKind();
}