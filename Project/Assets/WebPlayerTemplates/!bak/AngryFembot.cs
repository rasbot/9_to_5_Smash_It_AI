using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryFembot : MonoBehaviour {

    public GameObject[] glasses;

    private bool femPunched;
    private float speed_i;

    // Use this for initialization
    void Start () {
        speed_i = EnemyController._speed;
	}

    void AngryFemBot() // fembot will look mad and move faster
    {
        femPunched = true;
        EnemyController._speed = 2 * speed_i;
        glasses[0].SetActive(false);
        glasses[1].SetActive(true);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
