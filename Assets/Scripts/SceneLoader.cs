using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
	[SerializeField]
	TeamManager team;

	public void Load_Scene(string name) {
		SceneManager.LoadScene(name);
	}

	public void Exit_Dungeon_Scene() {
		team.Save_Unsaved_Changes();
	}
}
