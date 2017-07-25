using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour {
	[SerializeField]
	GameObject statsScreen;
	[SerializeField]
	Image statsPortrait;
	[SerializeField]
	Text characterName;
	[SerializeField]
	Text statsInfo;
	[SerializeField]
	ClickManager clickManager;

	CharacterObject focused;

	void Start() {
		toggle_screen(false);
		init_delegates();
	}

	#region delegate receiving
		void init_delegates() {
			clickManager.activate_choosing_attrib_target_event += show_attrib_screen;
			clickManager.deactivate_choosing_attrib_target_event += unshow_attrib_screen;

			clickManager.set_attrib_target_event += set_attrib_target;	
		}

		void set_attrib_target(CharacterObject charObj) {
			focused = charObj;
			update_screen();
		}

		void show_attrib_screen(CharacterObject charObj) {
			focused = charObj;
			toggle_screen(true);
			update_screen();
		}

		void unshow_attrib_screen(CharacterObject charObj) {
			toggle_screen(false);
		}
	#endregion

	#region screen methods
		void toggle_screen(bool value) {
			statsScreen.SetActive(value);
		}

		void update_screen() {
			string aux = "";
			aux += focused.data.nome.ToString().ToUpper();
			aux += "<color=gray>\n\n" + focused.data.classe.ToString().ToLower() + "</color>";
			characterName.text = aux;
			
			aux = "";
			aux += "HP: <color=gray>" + focused.health.hp + "/" + focused.status.getVIT();
			aux += "</color>\nFOR: <color=gray>" + focused.status.getFOR();
			aux += "</color>\nDEF: <color=gray>" + focused.status.getDEF();
			aux += "</color>\nINT: <color=gray>" + focused.status.getINT();
			aux += "</color>\nRES: <color=gray>" + focused.status.getRES();
			aux += "</color>\nAGI: <color=gray>" + focused.status.getAGI();
			aux += "</color>\nDES: <color=gray>" + focused.status.getDES() + "</color>";
			
			aux += "\n\n";
			for (int i = 0; i < focused.status.buffs.Count; i++) {
				aux += "<color=magenta>" + focused.status.buffs[i].ToString() + "</color> (" +
					focused.status.buffs[i].turnsLeft + " turns left) \n";
			}

			SlotBackground sb = focused.column.get_slotbg_by_charobj(focused);
			for (int i = 0; i < sb.slotBuff.buffs.Count; i++) {
				aux += "<color=magenta>" + sb.slotBuff.buffs[i].ToString() + "</color> (" +
					sb.slotBuff.buffs[i].turnsLeft + " turns left) \n";
			}

			statsInfo.text = aux;
		}
	#endregion
}
