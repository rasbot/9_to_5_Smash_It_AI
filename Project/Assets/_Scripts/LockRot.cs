using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRot : MonoBehaviour {

    Transform tf;

    private void Awake()
    {
        tf = transform;
    }
    
    // Update is called once per frame
    void LateUpdate () {
        Quaternion rot = tf.rotation;
        rot.x = 0;
        rot.y = 0;
        rot.z = 0;
        tf.rotation = rot;
    }
}
