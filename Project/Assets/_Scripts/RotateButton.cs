using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

  // TO TEST IN EDITOR!  ....
  // TURN OFF Event System : Gaze Input Module
  // TURN OFF PlayerController

 public class RotateButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
 {
 

	public int direction=1;
	public float speed = 1;

#if  !UNITY_EDITOR	
	public static float rotY = 0;
#else 
	public static float rotY = 0;//Ignored in EDITOR
#endif
	bool isPressed=false;

	public void OnPointerDown(PointerEventData eventData)
	{
		isPressed = true;
		//Cursor.visible = true;
		//Cursor.lockState = CursorLockMode.None;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
		print("back to normal!");
		//Cursor.visible = false;
		//      Cursor.lockState = CursorLockMode.Locked;
	}



	void Update()
	{
		if (isPressed)
		{
			rotY += direction * speed * Time.deltaTime;
			print("rotY: " + rotY);
		}
	}


}