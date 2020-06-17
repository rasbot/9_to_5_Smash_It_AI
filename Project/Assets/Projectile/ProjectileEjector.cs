using UnityEngine.Events;
using UnityEngine;

public class ProjectileEjector : MonoBehaviour {

    [SerializeField] GameObject projectilePrefab;
    public UnityEvent onCollision;
    public float velocity = 1f;
    public Vector3 localForward = Vector3.forward;
    [SerializeField] public float period = 0.5f;

	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("Eject", 0, period);	
	}

    void Eject()
    {
        GameObject proj = Instantiate(projectilePrefab);
        proj.transform.position = transform.position;
        proj.transform.rotation = transform.rotation;
        proj.GetComponent<Projectile>().Fire(this);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
