using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    [SerializeField]
    Animator damageAnimator;

    public IEnumerator Wait_For_Damage_Animation()
    {
        damageAnimator.gameObject.SetActive(true);

        damageAnimator.SetTrigger("show");

        RuntimeAnimatorController ac = damageAnimator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == "Attack_Damage_Effect")
            {
                yield return new WaitForSeconds(ac.animationClips[i].length);
            }
        }

        damageAnimator.gameObject.SetActive(false);
        yield break;
    }
}
