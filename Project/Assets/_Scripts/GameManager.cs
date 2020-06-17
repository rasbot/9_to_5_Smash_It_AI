using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public PlayerController player;
	public int storedScore;
	public int storedLives;
	public bool setGameManager;

	public static GameManager s;  
    public string currentSceneName;
    public bool tutLevel = false;

    void Awake()
	{
		setGameManager = false;
        player = GameObject.FindObjectOfType<PlayerController>();

	   if (s == null)
		{
			s = this;
			DontDestroyOnLoad(gameObject);
		} 
	   else
		{
			if (s != this)
			{
				Destroy(gameObject);    // make sure there is no other game manager!
			}
		}
	}


	public void InitializeValues()
	{
		storedScore = 0;
		storedLives = 2;        // give the player a life at the start of the game
	}
}

