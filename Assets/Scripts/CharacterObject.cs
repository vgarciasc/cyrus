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
	public SlotColumn column;
	public ButtonLane lane;
	public GameObject targetLowPriority;
	public GameObject targetHiPriority;

	//delegates
	public delegate void ClickDelegate(CharacterObject co);
	public event ClickDelegate click_event;

	Image img;

	void Start() {
		toggle_lane(false);
		toggle_targets(false);
	}

	#region setters
	public void set_data(int charID, CharacterData data) {
		this.charID = charID;
		this.data = data;
		init();
	}

	public void set_new_pos(Vector3 position) {
		this.transform.DOMove(position, 0.6f);
	}

	void init() {
		img = GetComponentInChildren<Image>();
		img.sprite = data.sprite;
	}
	#endregion

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

	#region death
	public void kill() {
		this.gameObject.SetActive(false);
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

	#region clicks
	public void register_click() {
		if (click_event != null) {
			click_event(this);
		}
	}
	#endregion

	#endregion
}
