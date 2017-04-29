using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour {	
	[HideInInspector]
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

		int getBuffAmount(BuffType type) {
			int amount = 0;

			for (int i = 0; i < buffs.Count; i++) {
				if (buffs[i].kind == type) {
					amount += buffs[i].amount;
				}
			}

			return amount;
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
			return character.data.VIT + getBuffAmount(BuffType.ATT_VIT);
		}
		
		public int getFOR() {
			return character.data.FOR + getBuffAmount(BuffType.ATT_FOR);
		}

		public int getDEF() {
			return character.data.DEF + getBuffAmount(BuffType.ATT_DEF);
		}

		public int getRES() {
			return character.data.RES + getBuffAmount(BuffType.ATT_RES);
		}

		public int getINT() {
			return character.data.INT + getBuffAmount(BuffType.ATT_INT);
		}

		public int getAGI() {
			return character.data.AGI + getBuffAmount(BuffType.ATT_AGI);
		}

		public int getDES() {
			return character.data.DES + getBuffAmount(BuffType.ATT_DES);
		}
	#endregion
}
