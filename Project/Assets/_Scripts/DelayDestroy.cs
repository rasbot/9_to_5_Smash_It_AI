using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroy : MonoBehaviour
{

    public float delay = 8f;


    private void Start()
    {
        Invoke("destroy", delay);
    }

    void destroy()
    {
        Destroy(gameObject);
    }

}
