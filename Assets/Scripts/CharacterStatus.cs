using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour {	
	
	class BuffNumbers {
		public int amount = 0;
		public float multiplier = 0;
	}

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

		BuffNumbers getBuffAmount(BuffType type) {
			BuffNumbers bnumbers = new BuffNumbers();

			for (int i = 0; i < buffs.Count; i++) {
				if (buffs[i].kind == type) {
					bnumbers.multiplier += buffs[i].multiplier;
					bnumbers.amount += buffs[i].amount;
				}
			}

			return bnumbers;
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
		public int getVIT() {
			return character.data.VIT + getBuffAmount(BuffType.ATT_VIT).amount;
		}
		
		public int getFOR() {
			return character.data.FOR + getBuffAmount(BuffType.ATT_FOR).amount;
		}

		public int getDEF() {
			return character.data.DEF + getBuffAmount(BuffType.ATT_DEF).amount;
		}

		public int getRES() {
			return character.data.RES + getBuffAmount(BuffType.ATT_RES).amount;
		}

		public int getINT() {
			return character.data.INT + getBuffAmount(BuffType.ATT_INT).amount;
		}

		public int getAGI() {
			return character.data.AGI + getBuffAmount(BuffType.ATT_AGI).amount;
		}

		public int getDES() {
			return character.data.DES + getBuffAmount(BuffType.ATT_DES).amount;
		}

		public float getBlockChance() {
			return getBuffAmount(BuffType.BLOCK_CHANCE).multiplier;
		}

		public float getCritMultiplier() {
			return getBuffAmount(BuffType.CRIT_MULTIPLIER).multiplier;
		}

		public AllyTarget getSwapTargets() {
			for (int i = 0; i < buffs.Count; i++) {
				Debug.Log(buffs[i].kind + ": " + buffs[i].swapTarget);
				if (buffs[i].kind == BuffType.SWAP_TARGETTING) {
					return buffs[i].swapTarget;
				}
			}

			return AllyTarget.ADJACENT; //the default
		}
	#endregion
}
