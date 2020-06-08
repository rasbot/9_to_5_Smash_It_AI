
using UnityEngine;

public class LookDetection : MonoBehaviour
{
    PuncherAgent puncher;

    void Start()
    {
        puncher = GetComponentInParent<PuncherAgent>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Target")
        {
            puncher.setIsLooking(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Target")
        {
            puncher.setIsLooking(false);
        }
    }
}
