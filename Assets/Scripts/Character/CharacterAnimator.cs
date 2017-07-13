using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour {

    public enum CharacterAnimatorEffects {
        STRIKE_DAMAGE,
        DAMAGE_NUMBER
    };

    [Header("STRIKE_DAMAGE")]
    [SerializeField]
    Animator strikeDamageAnimator;
    [Header("DAMAGE_NUMBER")]
    [SerializeField]
    Animator damageNumberAnimator;
    [SerializeField]
    Text damageNumberText;

    IEnumerator Wait_for_Animation_Effect(
        CharacterAnimatorEffects eff,
        string animationName) {

        Animator anim = Get_Animator_By_Enum(eff);

        anim.gameObject.SetActive(true);
        anim.SetTrigger("show");

        RuntimeAnimatorController ac = anim.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == animationName)
            {
                yield return new WaitForSeconds(ac.animationClips[i].length);
            }
        }

        anim.gameObject.SetActive(false);
        yield break;

    }

    Animator Get_Animator_By_Enum(CharacterAnimatorEffects eff) {
        switch (eff) {
            case CharacterAnimatorEffects.DAMAGE_NUMBER:
                return damageNumberAnimator;
            case CharacterAnimatorEffects.STRIKE_DAMAGE:
                return strikeDamageAnimator;
        }

        Debug.Log("Character Animator '" + eff.ToString() + "' not found.");
        return null;
    }

    public IEnumerator Wait_For_Strike_Damage_Animation() {
        yield return Wait_for_Animation_Effect(CharacterAnimatorEffects.STRIKE_DAMAGE,
            "Attack_Damage_Strike");
    }

    public IEnumerator Wait_For_Damage_Number_Animation(int damage_amount) {
        damageNumberText.text = damage_amount.ToString();
        yield return Wait_for_Animation_Effect(CharacterAnimatorEffects.DAMAGE_NUMBER,
            "Attack_Damage_Number");
    }
}
