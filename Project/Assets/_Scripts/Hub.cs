using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hub : MonoBehaviour {

    public static Hub s;// "s" (Singleton) we want to access Hub from anywhere

    public int someVar = 1;

	void Awake () {
        s = this;
	}

    //USE CASE:
    // print( Hub.s.someVar );

}
