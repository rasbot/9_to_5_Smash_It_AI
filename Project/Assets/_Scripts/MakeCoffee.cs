using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCoffee : MonoBehaviour {

    public GameObject coffeePrefab;
    private float randx;
    private float randz;
    private Vector3 pos;
    private bool oneCoffeePlease;
    private bool inRange;

    IEnumerator CoffeeDrop()
    {
		Glove.s.coffeeCreated = true;
        float rand = Random.Range(0.5f, 3.0f); // don't drop coffee instantly, have there be some variation
        yield return new WaitForSeconds(rand);
        if (inRange)
        {
            Vector3 coffeePos = new Vector3(transform.position.x + randx, transform.position.y, transform.position.z + randz);
            Instantiate(coffeePrefab, coffeePos, Quaternion.identity);
        }
    }

	// Use this for initialization
	void Start ()
    {
        inRange = false;
        randx = Random.Range(-4, 4);
        randz = Random.Range(0, 4);
        oneCoffeePlease = true;
	}


    void OnTriggerStay(Collider other)
    {
        if (other.tag == "CoffeeRadius")
        {
            inRange = true;
            if (PlayerController.s.coffeeCount < PlayerController.s.coffeeTOTAL && oneCoffeePlease) // only make coffee if we haven't made too many
            {
                StartCoroutine(CoffeeDrop());
                PlayerController.s.coffeeCount++;
                oneCoffeePlease = false;
            }
        }
        else inRange = false;
    }
}
