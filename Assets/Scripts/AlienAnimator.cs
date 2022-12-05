using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienAnimator : MonoBehaviour {

    public Animator animator;

	public void PlayAnim(string animName)
    {
		AnimatorClipInfo[] currentClips = animator.GetCurrentAnimatorClipInfo(0);
        for (int i = 0; i < currentClips.Length; i++)
        {
            if (currentClips[i].clip.name.Contains(animName))
            {
                return;
            }
        }
		animator.SetTrigger(animName);
	}
}
