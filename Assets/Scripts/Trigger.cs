using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerKind {
	ON_SUCCESSFUL_ATTACK, //triggered when player with skill makes a successful attack
	ON_PHYSICAL_ATTACK, //triggered when player with skill makes a physical attack
	ON_MAGICAL_ATTACK, //triggered when player with skill makes a magical attack
	ON_OWN_TURN_START, //triggered when the character's turn starts
	ON_START_MATCH, //triggered when match starts
	ON_SUCCESSFUL_CRIT //trigered when player with skills makes a successful attack and it is a critical hit
};

[System.Serializable]
public class Trigger : System.Object {
	public TriggerKind kind = new TriggerKind();
}