using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrapnel : MonoBehaviour {

    public static int maxShrapnels=10000;
    static List<Shrapnel> shrapnels = new List<Shrapnel>();

	static int cID;
	int id;

	bool collided=false;

	void Start () {
		cID++;
		id = cID;

		shrapnels.Add(this);
		print("Added: " + id);
	}


    private void OnDestroy()
    {
        shrapnels.Remove(this);
		print("Destroyed: " + id);
	}


    private void OnCollisionEnter(Collision collision)
    {
        collided=true;

        if (shrapnels.Count > maxShrapnels) {
            Shrapnel oldestCollided=getOldestCollided();
            if(oldestCollided) Destroy(oldestCollided.gameObject);
        }
    }

    Shrapnel getOldestCollided()
    {
        foreach(Shrapnel shrap in shrapnels) if(shrap.collided) return shrap;

        return null;
    }
}
