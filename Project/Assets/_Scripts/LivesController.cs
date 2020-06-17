using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesController : MonoBehaviour {

    public static LivesController s;

    public GameObject lifeGlovePrefab;    //prefab to instantiate

    public int livesNum;

    private bool oneLife;
    private bool oneDeath;
    private GameObject life;

    void Awake()
    {
        s = this;

    }

	void Start()
	{
		if (!GameManager.s.setGameManager)
		{
			GameManager.s.InitializeValues();
			GameManager.s.setGameManager = true;
		}
	}

	//IEnumerator SetLivesDelay()
	//{
	//}

	public void PlayerLives()
	{
        int num = GameManager.s.storedLives;
		//print("num: " + num);
        for (int i = 0; i < num; i++)  
        {
            GameObject lifeGlove = Instantiate(lifeGlovePrefab, Vector3.zero, Quaternion.identity);
            lifeGlove.transform.SetParent(gameObject.transform);
            lifeGlove.transform.localPosition = new Vector3(-0.25f * i, 0, 0);
        }
        oneLife = true;
        oneDeath = true;
        livesNum = transform.childCount;
	}

    public void AddLife()
    {
        if (oneLife)
        {
            life = Instantiate(lifeGlovePrefab, Vector3.zero, Quaternion.identity);
            life.transform.SetParent(gameObject.transform);
            life.transform.localPosition = new Vector3(-0.25f * livesNum, 0, 0);
        }

        oneLife = false;
    }

    public void LoseLife()
    {
        if (oneDeath)
        {
            Destroy(GetComponent<Transform>().GetChild(livesNum - 1).gameObject); //destroy outermost glove / life
            GameManager.s.storedLives = livesNum - 1; // store the number of lives 
        }
        oneDeath = false;
    }
}
