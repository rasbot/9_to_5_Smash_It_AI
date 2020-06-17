using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

// Attach this controller to the main camera, or an appropriate
// ancestor thereof, such as the "player" game object.
public class CameraController : MonoBehaviour {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN	
	public float speedH = 2.0f;
	public float speedV = 2.0f;

	private float yaw = 0.0f;
	private float pitch = 0.0f;

	void Update()
	{
		yaw += speedH * Input.GetAxis("Mouse X");
		pitch -= speedV * Input.GetAxis("Mouse Y");

		transform.eulerAngles = new Vector3(pitch, yaw, 0);
		// GameObject.FindGameObjectWithTag("dbText").GetComponent<Text>().text=""+RotateButton.rotY;

	}

#elif UNITY_ANDROID
	// Optional, allows user to drag left/right to rotate the world.
	private const float DRAG_RATE = .2f;
	float dragYawDegrees;

	void InitCamera()
	{
		//RotateButton.rotY += 10f;
		Camera.main.transform.rotation=Quaternion.identity;
	}

	void updateRot()
	{
			transform.localRotation =
		    // Allow user to drag left/right to adjust direction they're facing.
		    Quaternion.Euler (0f, -dragYawDegrees + RotateButton.rotY, 0f) *

		    // Neutral position is phone held upright, not flat on a table.
		    Quaternion.Euler (90f, 0f, 0f) *

		    // Sensor reading, assuming default `Input.compensateSensors == true`.
		    Input.gyro.attitude *

		    // So image is not upside down.
		    Quaternion.Euler (0f, 0f, 180f);
	}

	void Start () {
    // Make sure orientation sensor is enabled.
    Input.gyro.enabled = true;
	Invoke("InitCamera", 1f);
	//RotateButton.rotY = 90;
	//updateRot();
  }

  void Update () {
		//GameObject.FindGameObjectWithTag("dbText").GetComponent<Text>().text=""+RotateButton.rotY;
    if (XRSettings.enabled) {
      // Unity takes care of updating camera transform in VR.
      return;
    }

		// android-developers.blogspot.com/2010/09/one-screen-turn-deserves-another.html
		// developer.android.com/guide/topics/sensors/sensors_overview.html#sensors-coords
		//
		//     y                                       x
		//     |  Gyro upright phone                   |  Gyro landscape left phone
		//     |                                       |
		//     |______ x                      y  ______|
		//     /                                       \
		//    /                                         \
		//   z                                           z
		//
		//
		//     y
		//     |  z   Unity
		//     | /
		//     |/_____ x
		//

		// Update `dragYawDegrees` based on user touch.
		//CheckDrag ();
		
	updateRot();


		//void CheckDrag () {
		//  if (Input.touchCount != 1) {
		//    return;
		//  }

		//  Touch touch = Input.GetTouch (0);
		//  if (touch.phase != TouchPhase.Moved) {
		//    return;
		//  }

		//  dragYawDegrees += touch.deltaPosition.x * DRAG_RATE;
		//}

}
#endif
}
