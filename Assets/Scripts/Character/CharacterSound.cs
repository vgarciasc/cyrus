using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSound : MonoBehaviour {

	AudioSource source;

	[SerializeField]
	AudioClip damageStrike;

	void Start() {
		source = this.GetComponent<AudioSource>();
	}

	public void Play_Damage_Strike() {
		source.PlayOneShot(damageStrike);
	}
}
