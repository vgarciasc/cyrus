using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class CharacterObject : MonoBehaviour {
	public int charID = -1;
	public bool invertedSprite = false;

	[HideInInspector]
	public CharacterData data;
	[HideInInspector]
	public CharacterHealth health;
	[HideInInspector]
	public CharacterStatus status;
	[HideInInspector]
	public WeaponData weapon;
	[HideInInspector]
	public List<SkillData> skills = new List<SkillData>();
	
	[HeaderAttribute("Components")]
	public Image sprite;
	public SlotColumn column;
	public ButtonLane lane;
	public GameObject targetLowPriority;
	public GameObject targetHiPriority;
	public GameObject inactiveMask;

	//delegates
	public delegate void ClickDelegate(CharacterObject co);
	public event ClickDelegate click_event;

	int actions_left = 1;

	void Start() {
		health = this.GetComponent<CharacterHealth>();
		status = this.GetComponent<CharacterStatus>();

		toggle_lane(false);
		toggle_targets(false);
	}

	#region setters
		public void set_data(int charID, CharacterData data) {
			this.charID = charID;
			this.data = data;
			this.weapon = data.weapon;
			this.skills = data.skills;

			health.init(data);
			init();
		}

		public void set_new_pos(Vector3 position) {
			this.transform.DOMove(position, 0.6f);
		}

		void init() {
			sprite = sprite.GetComponentInChildren<Image>();
			sprite.sprite = data.sprite;
			init_delegates();
		}

		void init_delegates() {
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
		#region motions
			IEnumerator back_and_forth_motion(int distance, bool backwards, float duration) {
				int mod = 1;
				if (invertedSprite) mod *= -1;
				if (backwards) mod *= -1;

				transform.DOMoveX(this.transform.position.x + mod * distance, duration);
				yield return new WaitForSeconds(duration);
				transform.DOMoveX(this.transform.position.x - mod * distance, duration);
			}

			public void attack_motion() {
				StartCoroutine(back_and_forth_motion(20, false, 0.3f));
			}

			public void counter_attack_motion() {
				StartCoroutine(back_and_forth_motion(10, false, 0.2f));
			}
		#endregion motions
	
		public void take_hit(int amount) {
			health.add_health(- amount);

			if (health.hp == 0) {
				kill();
			}
		}

		public bool is_dead() {
			return health.hp <= 0;
		}

		public void kill() {
			this.gameObject.SetActive(false);
		}
	#endregion

	#region buffs
		public void take_buffs(List<Buff> buffs) {
			status.insert(buffs);
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
