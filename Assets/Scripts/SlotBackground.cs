using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SlotBackground : MonoBehaviour {
	public int id = -1;
	[SerializeField]
	Image background;
	[SerializeField]
	GameObject slotUpper;

	public Targettable target;
	public Buffable slotBuff;

	//delegates
	public delegate void ClickDelegate(int slotID);
	public event ClickDelegate click_event;

	//temp
	List<Color> colors = new List<Color>() {
		Color.red,
		Color.blue,
		Color.cyan,
		Color.magenta
	};

	public void set_new_pos(Vector3 position) {
		this.transform.DOMove(position, CharacterObject.swapWaitTime);
		slotUpper.transform.DOMove(position, CharacterObject.swapWaitTime);
	}

	public void set_ID(int ID) {
		this.id = ID;
		updateBackground();
	}

	void updateBackground() {
		if (id != -1) {
			this.background.color = colors [id];
		}
	}

	public void kill() {
		this.gameObject.SetActive(false);
		slotUpper.SetActive(false);
	}

	public void resize(int multiplier) { 
		this.transform.DOScaleY(multiplier, 0.6f);
	}

	public void register_click() {
		if (click_event != null) {
			click_event(id);
		}
	}
}
