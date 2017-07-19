using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DungeonExploratorTile : MonoBehaviour {
	
	[SerializeField]
	Image bottomImage;
	[SerializeField]
	Image rightImage;
	[SerializeField]
	Image upImage;
	[SerializeField]
	Image leftImage;
	
	bool bottom;
	bool right;
	bool up;
	bool left;

	public void Reset() {
		Set_Bottom(false);
		Set_Right(false);
		Set_Up(false);
		Set_Left(false);
	}

	public void Set_Bottom(bool value) {
		bottom = value;
		bottomImage.gameObject.SetActive(bottom);
	}

	public void Set_Right(bool value) {
		right = value;
		rightImage.gameObject.SetActive(right);
	}

	public void Set_Up(bool value) {
		up = value;
		upImage.gameObject.SetActive(up);
	}

	public void Set_Left(bool value) {
		left = value;
		leftImage.gameObject.SetActive(left);
	}

	public void Move(int direcao) {
		var exp = DungeonExplorator.Get_Dungeon_Explorator();
		exp.Move_Player((Direcao) direcao);
	}
}
