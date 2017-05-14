using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Active Skill Data")]
public class ActiveSkillData : ScriptableObject {
	public string name = "";
	public List<ElementActive> elements = new List<ElementActive>();
}
