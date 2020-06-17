using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kewb : MonoBehaviour {

    private Animator anim;

    IEnumerator AnimationToggle()
    {
        yield return new WaitForSeconds(3);
        anim.enabled = false;
        yield return new WaitForSeconds(2);
        anim.enabled = true;
    }
	// Use this for initialization
	void Start ()
    {
        anim = GetComponentInChildren<Animator>();
        anim.enabled = true;
        StartCoroutine(AnimationToggle());
	}
}
