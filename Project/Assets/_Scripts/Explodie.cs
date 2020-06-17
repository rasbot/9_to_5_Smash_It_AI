using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodie : MonoBehaviour {

    public float radius = 5.0f;
    public float power = 10.0f;
    public float upMod = 3.0f;

    // Use this for initialization
    void Start ()
    {
        Vector3 explosionPos = transform.position;
        Collider[] cols = Physics.OverlapSphere(explosionPos, radius);
        //float rand = Random.Range(-2.0f, 2.0f);
        //print("rand = " + rand);
        //Vector3 expVec = new Vector3(explosionPos.x + rand, explosionPos.y, explosionPos.z);
        //print("expVec = " + expVec);
        foreach (Collider col in cols)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(power, explosionPos, radius, upMod, ForceMode.Impulse);
            }
        }
    }
}
