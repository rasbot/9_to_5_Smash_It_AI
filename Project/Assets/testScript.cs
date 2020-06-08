using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        float result = 0;
        float value = 10*Random.value;
        if (value >= 5)
		{
            result = 1;
		}
        print(result);
    }
}
