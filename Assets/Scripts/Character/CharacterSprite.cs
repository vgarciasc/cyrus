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
    public Image fillFace;
    public Image lineHead;
	[Header("Paw Left")]
	public Image fillPawLeft;
	public Image linePawLeft;
	[Header("Paw Right")]
	public Image fillPawRight;
	public Image linePawRight;
	[Header("Weapon")]
	public Image weapon;

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
	
    public void Set_Race_Sprites(CharacterRace character_race) {
        string race_path = "Races\\";
        if (character_race == CharacterRace.CAT)  race_path += "Cat_Sprites_Data"; else
        if (character_race == CharacterRace.DOG)  race_path += "Dog_Sprites_Data"; else
        if (character_race == CharacterRace.BIRD) race_path += "Bird_Sprites_Data";

        RaceSpritesData sprites_data = Resources.Load<RaceSpritesData>(race_path);

        if (sprites_data != null){
            fillTail.sprite = sprites_data.fillTail;
            lineTail.sprite = sprites_data.lineTail;
            fillHead.sprite = sprites_data.fillHead;
            fillFace.sprite = sprites_data.fillFace;
            lineHead.sprite = sprites_data.lineHead;
        } else Debug.Log("Data não encontrada no race_path(" + race_path + ")");
    }

	public void Set_Random_Palette() {
		Color line = new Color(0.3f, 0.3f, 0.3f);
		Color fill = Color.white;

		fill = pool[Random.Range(0, pool.Count)];
		// fill += new Color(Random.Range(0.3f, 0.3f),
		// 	Random.Range(0.4f, 0.3f),
		// 	Random.Range(0.3f, 0.4f));

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

	public void Set_Weapon(ItemData data) {
		weapon.sprite = Resources.Load<Sprite>("Sprites\\Weapon\\" + data.sprite_name);
	}
}