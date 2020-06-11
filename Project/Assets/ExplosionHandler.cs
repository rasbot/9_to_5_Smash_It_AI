using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
	public GameObject explosions;
	public GameObject walker;
	public PuncherAgent puncher;

    void Start()
    {
        explosions.SetActive(false);
		puncher = GameObject.FindGameObjectWithTag("Puncher").GetComponent<PuncherAgent>();

    }

	IEnumerator Explosion()
	{
		explosions.SetActive(true);
		yield return new WaitForSeconds(2.0f);
		explosions.SetActive(false);
		walker.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Puncher"))
			StartCoroutine(Explosion());
	}
}
