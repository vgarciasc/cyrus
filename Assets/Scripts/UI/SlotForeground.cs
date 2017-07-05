using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotForeground : MonoBehaviour {
	public Animator groundAttack;

	public IEnumerator Ground_Attack_Animation() {
		groundAttack.gameObject.SetActive(true);

		groundAttack.SetTrigger("show");

		RuntimeAnimatorController ac = groundAttack.runtimeAnimatorController;
     	for (int i = 0; i < ac.animationClips.Length; i++)          
     	{
        	if (ac.animationClips[i].name == "GroundAttack_Attack")
        	{
				yield return new WaitForSeconds(ac.animationClips[i].length);
        	}
     	}
		
		groundAttack.gameObject.SetActive(false);
		yield break;
	}
}
