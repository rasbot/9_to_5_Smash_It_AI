using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour {

    public LayerMask collideWith;
    public float timeOut = 1f;
    ProjectileEjector ejector;
    bool isFired = false;

    public void Fire(ProjectileEjector ejector)
    {
        this.ejector = ejector;
        isFired = true;
        Destroy(gameObject, timeOut);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.Translate(ejector.localForward * ejector.velocity * Time.fixedDeltaTime);
	}

    void OnTriggerEnter(Collider other)
    {
        if (collideWith.Contains(other.gameObject.layer))
        {
            //print("other: " + other.gameObject.layer);
            //print("colide with: " + other.gameObject);
            ejector.onCollision.Invoke(); //Call ALL functions assigned to this event!!
        }
    }
}
