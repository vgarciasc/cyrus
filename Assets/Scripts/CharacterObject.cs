using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class CharacterObject : MonoBehaviour {
	public int charID = -1;

	[HideInInspector]
	public CharacterData data;
	
	[HeaderAttribute("References")]
	public Image sprite;
	public CharacterHealth health;
	public SlotColumn column;
	public ButtonLane lane;
	public GameObject targetLowPriority;
	public GameObject targetHiPriority;
	public GameObject inactiveMask;

	//delegates
	public delegate void ClickDelegate(CharacterObject co);
	public event ClickDelegate click_event;

	Image img;
	int actions_left = 1;

	void Start() {
		toggle_lane(false);
		toggle_targets(false);
	}

	#region setters
		public void set_data(int charID, CharacterData data) {
			this.charID = charID;
			this.data = data;
			
			health.init(data);
			init();
		}

		public void set_new_pos(Vector3 position) {
			this.transform.DOMove(position, 0.6f);
		}

		void init() {
			img = sprite.GetComponentInChildren<Image>();
			img.sprite = data.sprite;
		}
	#endregion

	#region actions
		public bool has_actions() {
			return actions_left > 0;
		}
		
		public void refresh_actions() {
			actions_left = 1;
			check_actions_empty();
		}

		public void use_action() {
			if (actions_left > 0) {
				actions_left--;
			}
			check_actions_empty();
		}

		void check_actions_empty() {
			if (actions_left == 0) {
				toggle_inactive_mask(true);
			}
			else {
				toggle_inactive_mask(false);
			}
		}
	#endregion

	#region UI access
		#region targeting
			public void toggle_targetlow(bool value) {
				targetLowPriority.SetActive(value);
			}

			public void toggle_targethi(bool value) {
				targetHiPriority.SetActive(value);
			}

			public void toggle_targets(bool value) {
				toggle_targetlow(value);
				toggle_targethi(value);
			}
		#endregion

		#region inactive mask
			void toggle_inactive_mask(bool value) {
				inactiveMask.SetActive(value);
			}

		#endregion

		#region lane
			public void toggle_lane() {
				//only one lane active at a time
				column.toggle_all_lanes(false);
				
				lane.toggle_cancel(!lane.cancelActive);
				lane.toggle_lane(!lane.laneActive);
			}

			public void toggle_lane(bool value) {
				//only one lane active at a time
				column.toggle_all_lanes(false);

				lane.toggle_cancel(value);
				lane.toggle_lane(value);
			}
		#endregion
	#endregion

	#region attack
		public void attack_motion() {
			StartCoroutine(attack_motion_());
		}

		IEnumerator attack_motion_() {
			transform.DOMoveX(this.transform.position.x - 20, 0.3f);
			yield return new WaitForSeconds(0.3f);
			transform.DOMoveX(this.transform.position.x + 20, 0.3f);
		}
	
		//returns true if dead
		public bool take_hit(int damage) {
			health.add_health(- damage);
			if (health.hp == 0) {
				kill();
				return true;
			}

			return false;
		}

		public void kill() {
			this.gameObject.SetActive(false);
		}
	#endregion

	#region clicks
		public void register_click() {
			if (click_event != null) {
				click_event(this);
			}
		}
	#endregion
}
