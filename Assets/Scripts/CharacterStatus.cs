using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : Buffable {	
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

	#region getters
		//caso o raposo comece a ter ideias mais esquisitas, isso aqui deve ser revisitado para 
		//ficar mais aos moldes de PassiveSkillManager.getCriticalProbability()

		public int getVIT() {
			SlotBackground sb = character.column.get_slotbg_by_charobj(character);
			return character.data.VIT + BuffManager.getBuffNumbers(buffs, BuffType.ATT_VIT).amount
				+ BuffManager.getBuffNumbers(sb.slotBuff.buffs, BuffType.ATT_VIT).amount;
		}
		
		public int getFOR() {
			SlotBackground sb = character.column.get_slotbg_by_charobj(character);
			return character.data.FOR + BuffManager.getBuffNumbers(buffs, BuffType.ATT_FOR).amount
				+ BuffManager.getBuffNumbers(sb.slotBuff.buffs, BuffType.ATT_VIT).amount;
		}

		public int getDEF() {
			SlotBackground sb = character.column.get_slotbg_by_charobj(character);
			return character.data.DEF + BuffManager.getBuffNumbers(buffs, BuffType.ATT_DEF).amount
				+ BuffManager.getBuffNumbers(sb.slotBuff.buffs, BuffType.ATT_VIT).amount;
		}

		public int getRES() {
			SlotBackground sb = character.column.get_slotbg_by_charobj(character);
			return character.data.RES + BuffManager.getBuffNumbers(buffs, BuffType.ATT_RES).amount
				+ BuffManager.getBuffNumbers(sb.slotBuff.buffs, BuffType.ATT_VIT).amount;
		}

		public int getINT() {
			SlotBackground sb = character.column.get_slotbg_by_charobj(character);
			return character.data.INT + BuffManager.getBuffNumbers(buffs, BuffType.ATT_INT).amount
				+ BuffManager.getBuffNumbers(sb.slotBuff.buffs, BuffType.ATT_VIT).amount;
		}

		public int getAGI() {
			SlotBackground sb = character.column.get_slotbg_by_charobj(character);
			return character.data.AGI + BuffManager.getBuffNumbers(buffs, BuffType.ATT_AGI).amount
				+ BuffManager.getBuffNumbers(sb.slotBuff.buffs, BuffType.ATT_VIT).amount;
		}

		public int getDES() {
			SlotBackground sb = character.column.get_slotbg_by_charobj(character);
			return character.data.DES + BuffManager.getBuffNumbers(buffs, BuffType.ATT_DES).amount
				+ BuffManager.getBuffNumbers(sb.slotBuff.buffs, BuffType.ATT_VIT).amount;
		}

		public AllyTarget getSwapTargets() {
			for (int i = 0; i < buffs.Count; i++) {
				if (buffs[i].kind == BuffType.SWAP_TARGETTING) {
					return buffs[i].swapTarget;
				}
			}

			SlotBackground slot = character.column.get_slotbg_by_charobj(character);
			for (int i = 0; i < slot.slotBuff.buffs.Count; i++) {
				if (slot.slotBuff.buffs[i].kind == BuffType.SWAP_TARGETTING) {
					return slot.slotBuff.buffs[i].swapTarget;
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
