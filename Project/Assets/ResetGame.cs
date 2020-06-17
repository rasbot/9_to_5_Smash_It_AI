using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour {


	public AudioClip youWin;
	public AudioSource audiosource;
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(EndGameMessage());
	}

	IEnumerator EndGameMessage()
	{
		yield return new WaitForSeconds(10);
		SceneManager.LoadScene(0);
	}
}
