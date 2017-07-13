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
    public CharacterAnimator characterAnimator;
	[HideInInspector]
	public CharacterSound audio;
    [HideInInspector]
	public WeaponData weapon;
	[HideInInspector]
	public bool is_blocking = false;

	public static float swapWaitTime = 0.6f;

	public List<PassiveSkillData> passiveSkills = new List<PassiveSkillData>();
	public List<ActiveSkillData> activeSkills = new List<ActiveSkillData>();

	[HeaderAttribute("Components")]
	public Image sprite;
	public SlotColumn column;
	public ButtonLane lane;
	public GameObject inactiveMask;
	public Label label;
	public Targettable target;
	public CharacterStatus status;

	//delegates
	public delegate void ClickDelegate(CharacterObject co);
	public event ClickDelegate click_event;

	int general_actions_left = 1;
	int swap_actions_left = 0;

	void Start() {
		init_references();

		toggle_lane(false);
		target.toggle_targets(false);
	}

	#region setters
	public void set_data(int charID, CharacterData data) {
		this.charID = charID;
		this.data = data;
		this.weapon = data.weapon;

		activeSkills.Clear();
		foreach (ActiveSkillData asd in data.skillsActive) {
			activeSkills.Add(Instantiate(asd));
		}

		passiveSkills.Clear();
		foreach (PassiveSkillData psd in data.skillsPassive) {
			passiveSkills.Add(Instantiate(psd));
		}

		health.init(data);
		init(data);
		init_delegates();
	}

	public void set_new_pos(Vector3 position) {
		this.transform.DOMove(position, swapWaitTime);
	}

	void init(CharacterData data) {
        gameObject.GetComponent<CharacterSprite>().Set_Race_Sprites(data.raca);
        //sprite = sprite.GetComponentInChildren<Image>();
        //sprite.sprite = data.sprite;
    }

	void init_references() {
		health = this.GetComponent<CharacterHealth>();
        characterAnimator = this.GetComponent<CharacterAnimator>();
		// status = this.GetComponent<CharacterStatus>();
	}

	void init_delegates() {
		TurnManager.getTurnManager().another_turn += pass_turn;
	}

	#endregion

	#region actions
		//swap actions is always current state of general actions plus the bonus

		public bool has_general_actions() {
			return general_actions_left > 0;
		}

		public bool has_swap_actions() {
			return swap_actions_left > 0;
		}
		
		public void refresh_actions() {
			general_actions_left = 1;

			swap_actions_left = general_actions_left + status.getSwapRefreshActions();

			check_actions_empty();
		}

		public void use_general_action() {
			if (general_actions_left > 0) {
				general_actions_left--;
			}
			if (swap_actions_left > 0) {
				swap_actions_left--;
			}
			check_actions_empty();
		}

		void check_actions_empty() {
			if (general_actions_left == 0) {
				toggle_inactive_mask(true);
			}
			else {
				toggle_inactive_mask(false);
			}
		}
	#endregion

	#region UI access
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
				yield return new WaitForSeconds(duration);
			}

			public IEnumerator attack_motion() {
				yield return StartCoroutine(back_and_forth_motion(20, false, 0.3f));
			}

			public IEnumerator counter_attack_motion() {
				yield return StartCoroutine(back_and_forth_motion(10, false, 0.2f));
            }

			Vector2 last_position_before_block;

			public IEnumerator block_attack_motion_start(CharacterObject toBlock) {
				is_blocking = true;

				int mod = 1;
				if (invertedSprite) mod *= -1;

				last_position_before_block = this.transform.position;

				transform.DOMoveX(this.transform.position.x + mod * 100, 0.3f);
				yield return new WaitForSeconds(0.3f);
				transform.DOMoveY(toBlock.transform.position.y, 0.3f);
				yield return new WaitForSeconds(0.3f);
			}

			public IEnumerator block_attack_motion_end() {
				is_blocking = false;

				transform.DOMoveY(last_position_before_block.y, 0.3f);
				yield return new WaitForSeconds(0.3f);
				transform.DOMoveX(last_position_before_block.x, 0.3f);
				yield return new WaitForSeconds(0.3f);

				last_position_before_block = Vector2.zero;

				yield break;
			}

			public IEnumerator dodge_motion() {
				transform.DOMoveY(this.transform.position.y + 15, 0.2f);
				yield return new WaitForSeconds(0.2f);
				transform.DOMoveY(this.transform.position.y - 15, 0.2f);
				yield return new WaitForSeconds(0.2f);
			}
		#endregion motions
	
		public void take_hit(int amount) {
			health.add_health(- amount);

			if (health.hp == 0) {
				column.kill_slot(this);
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
			if (status != null) {
				init_references();
				status.insert(buffs);
			}
		}
	#endregion

	#region clicks
		public void register_click() {
			if (click_event != null) {
				click_event(this);
			}
		}
	#endregion

	#region turns
		public void pass_turn() {
			foreach (ActiveSkillData act in activeSkills) {
				act.turnsSinceLastCast++;
			}			
		}
	#endregion

	#region heal
		public IEnumerator heal_by_buff(Buff buff) {
			yield return heal((int) (health.max * buff.multiplier));
		}

		public IEnumerator heal(int amount) {
			health.add_health(amount);
			yield return health.heal_anim();
		}
	#endregion
}
