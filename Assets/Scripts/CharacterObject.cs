using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class CharacterObject : MonoBehaviour {
	[HideInInspector]
	public CharacterData data;
	
	[HeaderAttribute("References")]
	public SlotColumn column;
	public ButtonLane lane;

	//delegates
	public delegate void ClickDelegate(CharacterObject co);
	public event ClickDelegate click_event;

	Image img;

	void Start() {
		toggle_lane(false);
	}

	#region setters
	public void set_data(CharacterData data) {
		this.data = data;
		init();
	}

	public void set_new_pos(Vector3 position) {
		this.transform.DOMove(position, 0.6f, false);
	}

	void init() {
		img = GetComponentInChildren<Image>();
		img.sprite = data.sprite;
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
		
		lane.toggle_lane(!lane.active);
	}

	public void toggle_lane(bool value) {
		//only one lane active at a time
		column.toggle_all_lanes(false);

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
