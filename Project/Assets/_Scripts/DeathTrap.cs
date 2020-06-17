using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrap : MonoBehaviour {

	public List<string> tags = new List<string>(){"Enemy"};
	public string deathFunctionName="DestroyStuckBot";

	void OnTriggerEnter(Collider other)
	{
		if(tags.Count==0 || tags.Contains(other.tag)){
			other.gameObject.SendMessage(deathFunctionName, SendMessageOptions.DontRequireReceiver);
			print(other.name+" hit the DeathTrap! Calling >> "+deathFunctionName);
		}
	}

}
