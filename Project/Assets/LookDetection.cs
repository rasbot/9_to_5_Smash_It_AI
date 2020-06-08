
using UnityEngine;

public class LookDetection : MonoBehaviour
{
    PuncherAgent puncher;

    bool isLooking=false;

    void Start()
    {
        puncher = GetComponentInParent<PuncherAgent>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Target")
        {
            isLooking = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Target")
        {
            isLooking = false;
        }
    }


    void FixedUpdate()
    {
        puncher.setIsLooking(isLooking);
    }
}
