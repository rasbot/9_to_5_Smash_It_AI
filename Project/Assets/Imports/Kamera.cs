using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera : MonoBehaviour {

    public GameObject kewb;

    void MakeKewbs()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(kewb, new Vector3(i * 2.0F, 0, 0), Quaternion.identity);
        }
    }
    // Use this for initialization
    void Start ()
    {
        MakeKewbs();
	}
}
