using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIController : MonoBehaviour {

	public static UIController s;

	[Header("Tutorial Level")]
	public GameObject player;
	public GameObject execuBot;
	public GameObject tutorialBot;
	public GameObject[] tutPos;
	public GameObject fader;
	public GameObject phoneHands;
    public GameObject[] tutorialText;

	private Animator execuAnim;
	public string currentSceneName;

	//private bool tutBotActive;
	private bool partOneEnded;
	private bool partTwoEnded;
	private bool partThreeEnded;
	private bool[] p = { true, true, true };
	private float failSaveDestroyTime;
	//private Animator floorAnim;

	[Header("All Levels")]
	public GameObject[] UIMessages;
	public GameObject[] endLevelUI;
	public Text[] endLevelText;
	public GameObject[] objectsDeactive;
	public GameObject[] BSODCracks;
	public GameObject waveUIContainer;
	public List<GameObject> waves = new List<GameObject>();
	public GameObject BSOD_Shattered;
    public GameObject BSODBackground;
    public GameObject livesContainer;
	public GameObject holePrefab;
	private GameObject spawnedTutBot;

	[Header("Audio")]
	public AudioClip[] audioClips;
	public AudioClip[] elevatorClips;
	public AudioClip[] BSODAudio;
	public AudioClip[] tutorial;
    public AudioClip death;
	public AudioSource audiosource;
	public bool elevatorAudio;
	public Animator eleAnim;

	private int toggleCount;

	public float botsDestroyed;
	private int totalScore;
	private bool slerpBool;

	//private Animator anim;
	private int bonusScore;
	private int bonusScore1;
	private int penaltyScore;
	//private bool once;
	public int totalBotsSpawnedThisWave = 0;
	private float thisLevelScore;
	private int tapCount;
	private bool tempBool;
	private bool doOnce;
	private bool lastTap;
    private bool playOnce = true;
	//private List<GameObject> BSODShatPieces = new List<GameObject>();
	private bool[] BSODaudioOnce = { true, true, true, true, true };
	public int holeTotal;
	public int totSpawnBot;
	void Awake()
	{
		s = this;
	}

	// Use this for initialization
	void Start()
	{
		//failSaveDestroyTime = 0;
		currentSceneName = SceneManager.GetActiveScene().name;
		tempBool = true;
		lastTap = true;
		doOnce = true;
		toggleCount = 0;
		elevatorAudio = false;
		tapCount = 0;
		//botsDestroyed = 0;
		DisableAllMessages();
		//once = false;
		//wave information
		if (waveUIContainer)
		{
			Transform flargen = waveUIContainer.GetComponent<Transform>();
			foreach (Transform go in flargen)
			{
				waves.Add(go.gameObject);
			}
		}
		StartCoroutine(UILevelStart());
		if (BSOD_Shattered)
		{
			BSOD_Shattered.SetActive(false);
            BSODBackground.SetActive(false);
        }
        if (currentSceneName == "L0_Tutorial")
        {
            Tutorial();
        }
		else 
		{
			LivesController.s.PlayerLives();  // only show player lives if NOT in tutorial
			//print("Populate Lives");
		}
	}
	//BSODShatPieces = BSOD_Shattered.GetComponentsInChildren<GameObject>();

	public void Tutorial()
	{
		partOneEnded = false;
		partTwoEnded = false;
		partThreeEnded = false;
		phoneHands.SetActive(false);
		execuAnim = execuBot.GetComponent<Animator>();
		PlayerController.s.glove.SetActive(false); // deactivate the glove at start (not really needed but whatevs)
		spawnedTutBot = null;
		StartCoroutine(TutorialLevelPart1());
	}


	//anim = fadeOutObjs.GetComponent<Animator>();

	public IEnumerator SlerpRot(GameObject slerpThis, Quaternion endRot, float slerpTime)
	{
		float elapsed = 0;
		while (elapsed < slerpTime)
		{
			elapsed += Time.deltaTime;
			Quaternion startRot = slerpThis.transform.localRotation;
			slerpThis.transform.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / slerpTime);

			yield return null;
		}
	}

    public IEnumerator SlerpRot(GameObject slerpThis, Vector3 endRot, float slerpTime)
    {
        float elapsed = 0;
        while (elapsed < slerpTime)
        {
            elapsed += Time.deltaTime;
            Vector3 startRot = slerpThis.transform.localEulerAngles;
            slerpThis.transform.localEulerAngles = Vector3.Slerp(startRot, endRot, elapsed / slerpTime);

            yield return null;
        }
    }

    IEnumerator TutorialLevelPart1()
	{		
        // start player at desk
        player.transform.position = tutPos[0].transform.position; // puts the player at the position near the desk
        fader.SetActive(true);
        yield return new WaitForSeconds(3);
        fader.SetActive(false);
        yield return new WaitForSeconds(6);
        audiosource.PlayOneShot(tutorial[0]);   // Greetings human.
        tutorialText[0].SetActive(true);
        yield return new WaitForSeconds(1.5f);
        tutorialText[0].SetActive(false);
        yield return new WaitForSeconds(1f);
        audiosource.PlayOneShot(tutorial[1]);   // Do you have what it takes to work at ZaamCore Enterprises?
        tutorialText[1].SetActive(true);
        yield return new WaitForSeconds(4.0f);
        tutorialText[1].SetActive(false);
        yield return new WaitForSeconds(1.0f);
        execuAnim.SetTrigger("TurnBot");
        audiosource.PlayOneShot(tutorial[2]);   // Please stand up and move 1.71 meters to your right.
        tutorialText[2].SetActive(true);
        yield return new WaitForSeconds(5.0f);
        tutorialText[2].SetActive(false);
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(PlayerController.s.LerpGameObject(player, tutPos[1].transform.position, 1.0f));
        yield return new WaitForSeconds(2);
        StartCoroutine(PlayerController.s.LerpGameObject(player, tutPos[2].transform.position, 2.0f));
        yield return new WaitForSeconds(2);

        audiosource.PlayOneShot(tutorial[3]);   // There is only one way to move up in this company - Smash the competition.
        tutorialText[3].SetActive(true);
        yield return new WaitForSeconds(5.0f);
        tutorialText[3].SetActive(false);
        yield return new WaitForSeconds(1.0f);
        audiosource.PlayOneShot(tutorial[4]);   // A fellow coworker wants your job promotion.
        tutorialText[4].SetActive(true);
        yield return new WaitForSeconds(3.0f);
        tutorialText[4].SetActive(false);
        //yield return new WaitForSeconds(1.0f);
        StartCoroutine(SpawnTutorialBot(tutorialBot, PlayerController.s.spawnLocations[0]));
        yield return new WaitForSeconds(3.0f);
        audiosource.PlayOneShot(tutorial[5]);   // Using the device in your hands, push forward quickly to neutralize your competitor.
        tutorialText[5].SetActive(true);
        PlayerController.s.glove.SetActive(true);
        StartCoroutine(SlerpRot(PlayerController.s.glove, Quaternion.identity, 2.0f));
        yield return new WaitForSeconds(2.0f);
        phoneHands.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        tutorialText[5].SetActive(false);
        phoneHands.SetActive(false);
        yield return new WaitForSeconds(1.0f);
		partOneEnded = true;
	}

	IEnumerator TutorialLevelPart2()
	{
        audiosource.PlayOneShot(tutorial[6]);   // Excellent work. You can punch slower or faster, depending on the speed your device moves.
        tutorialText[6].SetActive(true);
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(SpawnTutorialBot(tutorialBot, PlayerController.s.spawnLocations[0]));
        yield return new WaitForSeconds(3.0f);
        tutorialText[6].SetActive(false);
        partTwoEnded = true;
    }

	IEnumerator TutorialLevelPart3()
	{
        audiosource.PlayOneShot(tutorial[7]);   // Remember, hard working employees will move up faster in ZaamCore enterprises.
        tutorialText[7].SetActive(true);
        yield return new WaitForSeconds(6.0f);
        tutorialText[7].SetActive(false);
        StartCoroutine(SpawnTutorialBot(tutorialBot, PlayerController.s.spawnLocations[0]));
        yield return new WaitForSeconds(2.0f);
		partThreeEnded = true;
	}

	IEnumerator TutorialLevelPart4()
	{
        audiosource.PlayOneShot(tutorial[8]);   // You have what it takes to work at Zaam core. Congratulations human, you are hired.
        tutorialText[8].SetActive(true);
        yield return new WaitForSeconds(6.5f);
        tutorialText[8].SetActive(false);
        yield return new WaitForSeconds(1.5f);
        audiosource.PlayOneShot(tutorial[9]);   // Report to the mail room on the first floor.
        tutorialText[9].SetActive(true);
        yield return new WaitForSeconds(2.0f);
        tutorialText[9].SetActive(false);
        yield return new WaitForSeconds(1.0f);
		audiosource.PlayOneShot(tutorial[10]);  // Welcome aboard human.
        tutorialText[10].SetActive(true);
        yield return new WaitForSeconds(3.0f);
        tutorialText[10].SetActive(false);
        yield return new WaitForSeconds(2.0f);
		ElevatorOpen();
		yield return new WaitForSeconds(3.0f);
		StartCoroutine(PlayerController.s.LerpGameObject(player, tutPos[3].transform.position, 3.0f));
		yield return new WaitForSeconds(3.0f);
		ElevatorClose();
		Vector3 gloveDownRot = new Vector3(90, 0, 0);
		StartCoroutine(SlerpRot(PlayerController.s.glove, gloveDownRot, 2.0f));
		yield return new WaitForSeconds(5.0f);
		PlayElevatorMusic();
		yield return new WaitForSeconds(5.0f);
		GameManager.s.tutLevel = false;
        LoadNextLevel();
		//LoadLevel("L1_MailRoom");
	}

	IEnumerator SpawnTutorialBot(GameObject bot, Transform spawnLocation)       // only used for tutorial bot...it got weird using the other method
	{
		//yield return new WaitForSeconds(0.5f);
		spawnedTutBot = Instantiate(bot, spawnLocation.position, spawnLocation.rotation);
		Vector3 holePos = new Vector3(spawnLocation.position.x, 3.405f, spawnLocation.position.z); // y position is hard coded so it's always on the floor
		GameObject spawnedHole = Instantiate(holePrefab, holePos, Quaternion.identity);
        spawnedTutBot.GetComponent<Collider>().enabled = false;
        spawnedTutBot.GetComponent<Rigidbody>().AddForce(Vector3.up * 13, ForceMode.Impulse);
		yield return new WaitForSeconds(1.0f);
        spawnedTutBot.GetComponent<Collider>().enabled = true;
		yield return new WaitForSeconds(0.5f);
		Destroy(spawnedHole);
	}

	void DisableAllMessages()
	{
		foreach (GameObject go in UIMessages)
		{
			go.SetActive(false);
		}
		foreach (GameObject go in endLevelUI)
		{
			go.SetActive(false);
		}
	}

	public void ResetScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadLevel(string sceneName)
	{
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(sceneName);
		botsDestroyed = 0;
	}

    public void LoadNextLevel()
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		botsDestroyed = 0;
        //SceneManager.LoadScene(sceneName);
    }

    void RestartGame()
	{
		SceneManager.LoadScene(0);
		botsDestroyed = 0;
	}

	IEnumerator DeathReset()
	{
		LivesController.s.LoseLife();     // take a life away
		yield return new WaitForSeconds(4);
		ResetScene();
	}

	IEnumerator UILevelStart()
	{
		yield return new WaitForSeconds(4);
		UIMessages[0].SetActive(true);
		yield return new WaitForSeconds(3f);
		UIMessages[0].SetActive(false);
	}

	IEnumerator EndLevelUIMessages()
	{
		int kFinalScoreText=0;
		int kBotsDest=1;
		int kBotPerc=2;

        yield return new WaitForSeconds(3);     // wait a little before showing end level UI messages
		endLevelUI[kFinalScoreText].SetActive(true);          // show score
		endLevelText[kFinalScoreText].text = "SCORE: " + PlayerController.s.score.commas();
		audiosource.PlayOneShot(audioClips[0]);
		yield return new WaitForSeconds(1);

		endLevelUI[kBotsDest].SetActive(true);          // bots destroyed:
		audiosource.PlayOneShot(audioClips[0]);
		yield return new WaitForSeconds(0.5f);
		endLevelUI[kBotPerc].SetActive(true);          // show percent
		float total = PlayerController.s.totalBotsSpawned + totalBotsSpawnedThisWave;
		float perc = botsDestroyed / total;
		print("(UI) bots Destroyed: " + botsDestroyed);
		print("(UI) bots Spawned: " + total);
		print("(UI) perc: " + perc);
		endLevelText[kBotPerc].text = (Mathf.RoundToInt(perc * 100) + "%");
		audiosource.PlayOneShot(audioClips[0]);
		//yield return new WaitForSeconds(1);

		//BONUS SCORE : Text
		//bonusScore = Mathf.RoundToInt(thisLevelScore * perc);
		//endLevelText[kBonusScore].text = "" + bonusScore.commas();

		//yield return new WaitForSeconds(1);
		//endLevelUI[1].SetActive(false);         // bonus value turn off
		//totalScore = PlayerController.s.score + bonusScore;
		//endLevelText[0].text = "SCORE: " + totalScore.commas();    // update score
		//audiosource.PlayOneShot(audioClips[0]);
		//yield return new WaitForSeconds(1);


		//if (endLevelUI.Length <= 5)
		//{
		//	yield return new WaitForSeconds(4);
		//	GameManager.storedScore = totalScore;
		//	LoadLevel();

		//}
		//else
		//{
			//if (PlayerController.s.powerupsCollected > 0)
			//{
			//endLevelUI[5].SetActive(true);                  // PU used
			//yield return new WaitForSeconds(0.5f);
			//endLevelUI[6].SetActive(true);                  // PU number
			//endLevelText[6].text = "" + (PlayerController.s.powerupsCollected);
			////yield return new WaitForSeconds(1);
			////endLevelUI[1].SetActive(true);                  // bonus score
			////bonusScore1 = Mathf.RoundToInt(thisLevelScore * 0.15f * (PlayerController.s.powerupsCollected));
			////endLevelText[1].text = "bonus = " + bonusScore1.commas();
			////yield return new WaitForSeconds(1);
			////endLevelUI[1].SetActive(false);                 // bonus value turn off
			////totalScore += bonusScore1;
			////endLevelText[0].text = "SCORE: " + totalScore.commas();    // update score
			////audiosource.PlayOneShot(audioClips[0]);
			////yield return new WaitForSeconds(1);
			//}

			//if (Glove.s.healthPacksCollected > 0)
			//{
			//endLevelUI[7].SetActive(true);                  // HP used
			//yield return new WaitForSeconds(0.5f);
			//endLevelUI[8].SetActive(true);                  // HP number
			//endLevelText[8].text = "" + Glove.s.healthPacksCollected;
			//yield return new WaitForSeconds(1);
			//}
			//endLevelUI[2].SetActive(true);                  // penalty score
			//penaltyScore = Mathf.RoundToInt(thisLevelScore * 0.25f * Glove.healthPacksCollected);
			//endLevelText[2].text = "penalty = " + penaltyScore.commas();
			//yield return new WaitForSeconds(1);
			//endLevelUI[2].SetActive(false);                 // penalty value turn off
			//totalScore -= penaltyScore;
			//endLevelText[0].text = "SCORE: " + totalScore.commas();    // update score
			yield return new WaitForSeconds(5);
			GameManager.s.storedScore = totalScore;
			PlayerController.s.totalBotsSpawned = 0; // only reset this at the end of a level before next level is loaded
			LoadNextLevel();
		//}
	}

	bool eleOpenOnce = true;
	public void ElevatorOpen() // 5 second animation
	{
		//StartCoroutine(ElevatorAudioTiming());
		audiosource.Stop();                         // stop the elevator music
		audiosource.PlayOneShot(elevatorClips[0]);  // play audio of door ding
		eleAnim.SetTrigger("OpenDoor");
		audiosource.PlayOneShot(elevatorClips[1]);  // play audio of door opening
		eleOpenOnce = false;
	}

	bool eleCloseOnce = true;
	public void ElevatorClose()  // 3.5 second animation
	{
		eleAnim.SetTrigger("CloseDoor");            // play animation of door closing
		audiosource.PlayOneShot(elevatorClips[2]);  // play audio of door closing
		eleCloseOnce = false;
	}

	public void PlayElevatorMusic()
	{
		audiosource.PlayOneShot(elevatorClips[3]);  // play elevator music
	}
	public void BSODAudioPlay(int i)
	{
		audiosource.PlayOneShot(BSODAudio[i]);
		BSODaudioOnce[i] = false;
	}
	IEnumerator NextWaveSequence()
	{
		yield return new WaitForSeconds(3);
		if (waves[PlayerController.s.waveNumber - 1] != null)
		{
			waves[PlayerController.s.waveNumber - 1].SetActive(true);
		}
		PlayerController.s.spawnBots = true;
		PlayerController.s.ResetVariables();
		yield return new WaitForSeconds(3);
		waves[PlayerController.s.waveNumber - 1].SetActive(false);
	}
	public void NextWaveUISequence()
	{
		StartCoroutine(NextWaveSequence());
	}
	public void StartEndMessages()
	{
		StartCoroutine(EndLevelUIMessages());
	}

	IEnumerator TappedOut()
	{
		//foreach (GameObject go in BSODShatPieces)   // turn on shattered stuff
		//{
		//	go.SetActive(true);
		//}
		foreach (GameObject go in BSODCracks)       // turn off cracks
		{
			go.SetActive(false);
		}
		UIMessages[1].SetActive(false);              // turn off not shattered BSOD
		BSOD_Shattered.SetActive(true);
        BSODBackground.SetActive(true);
		Color tmp = BSODBackground.GetComponent<Renderer>().materials[0].color;
		tmp.a = 1f;
		BSODBackground.GetComponent<Renderer>().materials[0].color = tmp;

		//foreach (GameObject go in BSODShatPieces)
		//{
		//    go.GetComponent<Collider>().enabled = true;
		//    go.GetComponent<Rigidbody>().isKinematic = false;
		//}
		BSODAudioPlay(4);
		// blow up BSOD, wait a sec, then restart game
		yield return new WaitForSeconds(5);
		RestartGame();
	}

	// Update is called once per frame
	void Update ()
	{
        //print("tutBot[0] = " + tutBot[0]);
        if (currentSceneName == "L0_Tutorial")
		{
			if (spawnedTutBot != null)  // a bot is active
			{
				//print("bot name: " + spawnedTutBot.name);
                // don't do anything 
			}
			else if (partOneEnded) 
			{
				StartCoroutine(TutorialLevelPart2());
				partOneEnded = false;
			}

			else if (partTwoEnded)
			{
				StartCoroutine(TutorialLevelPart3());
				partTwoEnded = false;
			}
			else if (partThreeEnded)
			{
				StartCoroutine(TutorialLevelPart4());
				partThreeEnded = false;
			}
		}


		if (Input.GetMouseButtonDown(0)) tapCount++;

		if (PlayerController.s.playerDead)        // player died
		{
            if (playOnce)
            {
                audiosource.PlayOneShot(death);       // play death sound
                playOnce = false;
            }

			if (LivesController.s.livesNum > 0)   // we have lives left, reset scene
			{
				StartCoroutine(DeathReset());
                PlayerController.s.totalBotsSpawned = 0; // reset on death since the entire level is being reset
			}
			else
			{
				if (tempBool)
				{
					tapCount = 0;
					tempBool = false;
				}

				//int tapCountPrev = tapCount;
				//tapCountLocal = tapCount - tapCountPrev; // when player dies, start counting taps
				if (doOnce)
				{
					UIMessages[1].SetActive(true);
					foreach (GameObject go in objectsDeactive)  // turn off the glove and score because they block text from the BSOD
					{
						if(go) go.SetActive(false);
					}
					doOnce = false;
				}

				if (tapCount > 0 && tapCount != 5)
				{
					BSODCracks[tapCount - 1].SetActive(true);  // turn on a crack for each tap
					if (BSODaudioOnce[tapCount - 1])
					{
						BSODAudioPlay(tapCount - 1);
					}
					//audiosource.PlayOneShot(BSODAudio[tapCount - 1]);
				}
				else if (tapCount == 5 && lastTap)
				{
					StartCoroutine(TappedOut());
					lastTap = false;
				}
				else tapCount = 0; // this is only so I don't get errors for the index being out of range

				//StartCoroutine(GameOver());     // no lives left...restart game!
			}
		}
	}
}
