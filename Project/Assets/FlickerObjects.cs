using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerObjects : MonoBehaviour {

	public Material mymat;
	public GameObject FlickerObjectParent;

	// Use this for initialization
	void Start () 
	{
	 mymat = GetComponent<Renderer>().material;	
	}

	IEnumerator Flicker()
	{
	FlickerObjectParent.SetActive(false);
    mymat.SetColor("_EmissionColor", Color.red);
	yield return new WaitForSeconds(1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
