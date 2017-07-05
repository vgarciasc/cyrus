using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum CharacterRace { CAT, DOG, BIRD };
public class CharacterSprite : MonoBehaviour {

	[Header("Tail")]
	public Image fillTail;
	public Image lineTail;
	[Header("Body")]
	public Image fillBody;
	public Image lineBody;
	[Header("Head")]
	public Image fillHead;
	public Image lineHead;
	[Header("Paw Left")]
	public Image fillPawLeft;
	public Image linePawLeft;
	[Header("Paw Right")]
	public Image fillPawRight;
	public Image linePawRight;

	List<Image> fills = new List<Image>();
	List<Image> lines = new List<Image>();

	[Header("Possible Colors")]
	public List<Color> pool = new List<Color>() {
		Color.red,
		Color.yellow,
		Color.gray,
		Color.cyan,
		Color.blue
	};

	void Start() {
		fills = new List<Image>() {fillTail, fillBody, fillHead, fillPawLeft, fillPawRight};
		lines = new List<Image>() {lineTail, lineBody, lineHead, linePawLeft, linePawRight};

		Set_Random_Palette();
	}
	
	public void Set_Random_Palette() {
		Color line = Color.black;
		Color fill = Color.white;

		fill = pool[Random.Range(0, pool.Count)];
		fill += new Color(Random.Range(0.3f, 0.3f),
			Random.Range(0.4f, 0.3f),
			Random.Range(0.3f, 0.4f));

		Set_Fill_Color(fill);
		Set_Line_Color(line);

		if (Random.Range(0, 10) == 7) {
			Set_Body_Color(Get_Random_Pool_Color());
		}
	}

	void Set_Fill_Color(Color color) {
		foreach (Image img in fills) {
			img.color = color;
		}
	}

	void Set_Line_Color(Color color) {
		foreach (Image img in lines) {
			img.color = color;
		}
	}

	void Set_Body_Color(Color color) {
		fillBody.color = color;
	}

	Color Get_Random_Pool_Color() {
		return pool[Random.Range(0, pool.Count)];
	}
}