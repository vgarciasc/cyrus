using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour {	
	public List<Buff> buffs = new List<Buff>();

	[HeaderAttribute("References")]
	[SerializeField]
	TurnManager turnManager;

	CharacterObject character;

	void Start() {
		character = this.GetComponent<CharacterObject>();

		turnManager.another_turn += pass_turn;
	}

	void pass_turn() {
		update_buff();
	}

	#region buff
		void update_buff() {
			for (int i = 0; i < buffs.Count; i++) {
				if (buffs[i].turnsLeft != -1) {
					buffs[i].turnsLeft--;
					
					if (buffs[i].turnsLeft == 0) {
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
				}
			}
		}

		bool hasBuff(Buff buff) {
			for (int i = 0; i < buffs.Count; i++) {
				if (buffs[i].Equals(buff)) {
					return true;
				}
			}	

			return false;
		}
	#endregion

	#region getters
		//caso o raposo comece a ter ideias mais esquisitas, isso aqui deve ser revisitado para 
		//ficar mais aos moldes de PassiveSkillManager.getCriticalProbability()

		public int getVIT() {
			return character.data.VIT + BuffManager.getBuffNumbers(buffs, BuffType.ATT_VIT).amount;
		}
		
		public int getFOR() {
			return character.data.FOR + BuffManager.getBuffNumbers(buffs, BuffType.ATT_FOR).amount;
		}

		public int getDEF() {
			return character.data.DEF + BuffManager.getBuffNumbers(buffs, BuffType.ATT_DEF).amount;
		}

		public int getRES() {
			return character.data.RES + BuffManager.getBuffNumbers(buffs, BuffType.ATT_RES).amount;
		}

		public int getINT() {
			return character.data.INT + BuffManager.getBuffNumbers(buffs, BuffType.ATT_INT).amount;
		}

		public int getAGI() {
			return character.data.AGI + BuffManager.getBuffNumbers(buffs, BuffType.ATT_AGI).amount;
		}

		public int getDES() {
			return character.data.DES + BuffManager.getBuffNumbers(buffs, BuffType.ATT_DES).amount;
		}

		public AllyTarget getSwapTargets() {
			for (int i = 0; i < buffs.Count; i++) {
				if (buffs[i].kind == BuffType.SWAP_TARGETTING) {
					return buffs[i].swapTarget;
				}
			}

			return AllyTarget.ADJACENT; //the default
		}

		public bool getSwappable() {
			for (int i = 0; i < buffs.Count; i++) {
				Debug.Log(buffs[i].kind + ": " + buffs[i].swapTarget);
				if (buffs[i].kind == BuffType.SWAP_BLOCKING) {
					return false;
				}
			}

			return true;
		}
	#endregion
}
