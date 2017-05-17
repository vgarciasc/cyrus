using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffable : MonoBehaviour {
	[HideInInspector]
	public List<Buff> buffs = new List<Buff>();

	public Barrier barrier;
	public BuffContainer container;
	public Targettable target;

	protected void update_buff() {
		for (int i = 0; i < buffs.Count; i++) {
			if (buffs[i].turnsLeft != -1) {
				buffs[i].turnsLeft--;
				
				if (buffs[i].turnsLeft == 0) {
					if (buffs[i].kind == BuffType.BARRIER) {
						barrier.add_health(buffs[i].amount);
					}
					if (buffs[i].kind == BuffType.DELAYED_ATTACK) {
						//TODO: como não é yield return vai dar problema se tiver animação
						// target.GetCharacter().health.(buffs[i].amount);
						Debug.Log("Take damage amount : " + buffs[i].amount);
					}
					buffs.RemoveAt(i);
				}
			}
		}
	}

	public void insert(List<Buff> buffs) {
		for (int i = 0; i < buffs.Count; i++) {
			if (hasBuff(buffs[i]) && !buffs[i].stackable) {
				continue;
			}
			else {
				this.buffs.Add(new Buff(buffs[i]));
				if (buffs[i].kind == BuffType.BARRIER) {
					barrier.add_health(buffs[i].amount);
				}
			}
		}
	}

	protected bool hasBuff(Buff buff) {
		for (int i = 0; i < buffs.Count; i++) {
			if (buffs[i].Equals(buff)) {
				return true;
			}
		}	

		return false;
	}
}
