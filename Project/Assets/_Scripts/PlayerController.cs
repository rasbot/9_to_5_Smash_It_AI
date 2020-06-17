using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController s;

    //spawned objects
    public GameObject[] botPrefabs;
    public GameObject[] PUpPrefabs;
    public GameObject[] holePrefabs;
    public GameObject healthPrefab;
    public GameObject spawnLocationsContainer;
    public GameObject holeLocationsContainer;
    public GameObject powerUpLocationsContainer;
    public GameObject healthPackLocationsContainer;
    public int botTypeIndex;

    //instantiate information
    [Header("Instantiate Coefficients")]
    public float[] botSpawnWeights = new float[6]; // 6 bots - mailbot, clampbot, coffeebot, fembot, destructobot, thermobot
    public float decayFactor;
    public float coefficent;
    public int botTotal;
    public int botsSpawnedThisWave;
    public int instantiateInt;
    [Space(10)]
    public GameObject glove;
    
	private GameObject explode;
    public bool powerUpInGame;       // is there a powerup somewhere?
    [Header("PowUp Health Info")]
    public float PUpTimeLength;
    public float pupTime;
    public int powerupTotal;
    public int powerupTOTAL;
    public int coffeeTotal;
    public int coffeeTOTAL;
    public int healthTotal;
    public int healthTOTAL;
    public Animator glovePunchAnim;
    private Collider[] elecHitColliders;
    //public static Collider[] elecHitEnemy;
    private int puIndex;
    private float spawnTime;
    private bool waitForSpawnTime;
    public int powerupsSpawned;
    public int coffeeCount;

    //Player Health
    [Header("Player Health")]
    public float currentHealth;
    public float maxHealth = 100f;
    public Slider healthBar;
    public GameObject playerHitFlash;
    public float instantiateTime;
	public Text playerScore;
    public int score;
    public GameObject healthIncreasePoof;
    [Space(10)]
    //Start Button
    public GameObject startButton;
    public GameObject startLetters;

    //UI Controller
    public GameObject movePlayerHere;
	public int totalBotsSpawned;
    private Vector3 position_i;
    private float t;
    private float timeToMove;   // time to reach player position
    private float thisLevelScore;
    private bool noMoreWaves;

    //Make bots explode
    public bool spawnBots;
    private Collider[] colChildren;
    private Rigidbody[] rigidChildren;
    private Collider startCol;
    public AudioClip explosionAudio;
    public AudioSource audiosource;

    public bool playerDead;

    public string deathTriggerName;
    public Animator playerHitAnim;

    private Vector3 origin;
    public float mouseTimeDown;
    public float gloveAnimSpeed;
    public float accelerationZ;
    private float accelerationY;
    public string currentSceneName;
    private GameObject resetSceneButton;
    public List<Transform> spawnLocations = new List<Transform>();
    public List<Transform> holeLocations = new List<Transform>();
    public List<Transform> tmp = new List<Transform>();
    public List<Transform> PUpLocations = new List<Transform>();
    public List<Transform> healthLocations = new List<Transform>();
    private GameObject powerup;
    private GameObject health;
    private bool needHealth;
    private bool coffeePunch;
    private int botSpawnIndex;
    private bool slerpBool;
    public bool levelCompleted;
    public bool waveCompleted;
    public int waveTotal;
    public bool holeBot;
    public int waveNumber;
    public bool resetVariables;
    public float holeTotal;
    private bool once1;
    private bool once2;
    private int[] pUpCount = new int[] { 0, 0, 0 };  // used to set which powerups are allowed to spawn
    public GameObject spawnedHole;
    public Collider botCol;
    public Rigidbody rb;
    public float holeForceMag;
    private float _holeForceMag;
    private Vector3 holePos;

    void Awake()
    {
        s = this;
    }

    // Use this for initialization
    void Start()
    {
		GameObject explodeParent=GameObject.Instantiate( (GameObject) Resources.Load("EffectsSettingsContainer"));
		explodeParent.transform.parent=Camera.main.transform;
		explodeParent.transform.localPosition = Vector3.zero;
		explodeParent.transform.localRotation=Quaternion.identity;
		explode=explodeParent.transform.GetChild(0).gameObject;

        playerHitFlash.SetActive(false);
        resetSceneButton = GameObject.FindGameObjectWithTag("ResetSceneButton");
        //if (SceneManager.GetActiveScene().name == "L0_Tutorial") resetSceneButton.SetActive(true);
        //else if(resetSceneButton) resetSceneButton.SetActive(false);
        _holeForceMag = holeForceMag;
        once1 = true;
        once2 = false;
        resetVariables = false;
        waveNumber = 1;         // should start at wave 1 for every level TEMP
        slerpBool = false;
        noMoreWaves = false;
        position_i = transform.position;
        coffeeCount = 0;
        pupTime = PUpTimeLength;
        powerupsSpawned = 0;
        waitForSpawnTime = false;
        spawnTime = botTotal;           // wait this long to spawn the first powerup
        puIndex = 0;                    // make the first powerup spawn at the first location
        //powerupTOTAL = powerupTotal;    
        //botTOTAL = botTotal;            
        //coffeeTOTAL = coffeeTotal;      
        //healthTOTAL = healthTotal;      
        levelCompleted = false;
        waveCompleted = false;
        botSpawnIndex = 0;
        botsSpawnedThisWave = 0;
        coffeePunch = true;             // used so only one coffee is spawned at a time
        powerUpInGame = false;          // is there a powerup somewhere?
        needHealth = true;
        explode.SetActive(false);
        //healthIncreasePoof.SetActive(false);
        spawnBots = false;
        rigidChildren = startLetters.GetComponentsInChildren<Rigidbody>();
        colChildren = startLetters.GetComponentsInChildren<Collider>();
        startCol = startButton.GetComponent<Collider>();
        currentSceneName = SceneManager.GetActiveScene().name;
        instantiateInt = 0;
        instantiateTime = 10000;
        //create lists of spawn locations
        //enemy spawn locations
        for (int childIndex = 0; childIndex < spawnLocationsContainer.transform.childCount; ++childIndex)
        {
            spawnLocations.Add(spawnLocationsContainer.transform.GetChild(childIndex));
        }
        //hole spawn locations
        if (holeLocationsContainer)
        {
			//if( holeLocationsContainer.transform.childCount == spawnLocations.Count)
			//{
			
				for (int childIndex = 0; childIndex < holeLocationsContainer.transform.childCount; ++childIndex)
				{
					Transform holeTF = holeLocationsContainer.transform.GetChild(childIndex);
					holeLocations.Add(holeTF);
				}
			//}
			//else
			//{
				//Debug.LogError("OOPS!  holeLocations is not same count as spawnLocations!");
			//}
        }
        //power up spawn locations
        for (int childIndex = 0; childIndex < powerUpLocationsContainer.transform.childCount; ++childIndex)
        {
            PUpLocations.Add(powerUpLocationsContainer.transform.GetChild(childIndex));
        }
        //health pack spawn locations
        for (int childIndex = 0; childIndex < healthPackLocationsContainer.transform.childCount; ++childIndex)
        {
            healthLocations.Add(healthPackLocationsContainer.transform.GetChild(childIndex));
        }
        //shuffle lists
        Shuffle(spawnLocations);
        Shuffle(holeLocations);
        Shuffle(PUpLocations);
        Shuffle(healthLocations);

        mouseTimeDown = 0;
        glovePunchAnim = glove.GetComponentInChildren<Animator>();
        glove.SetActive(false);
        glove.transform.localEulerAngles = new Vector3(90, 0, 0);   // make sure the glove is not pointed up
        playerDead = false;
        Vector3 origin = Vector3.zero;
        UpdateHealth(maxHealth);
        score = GameManager.s.storedScore; // load score from last level
        UpdateScore();


		
        if (currentSceneName != "L0_Tutorial")
        {
        // initialize level and wave variables from UI script
        ResetVariables();
        StartCoroutine(MoveToPosition());                       // move player to starting position only if not in the tutorial level
        }
        powerupTOTAL = powerupTotal; 
        coffeeTOTAL = coffeeTotal;
        healthTOTAL = healthTotal;
    }

    void Shuffle(List<Transform> list)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int i = 0; i < list.Count; i++)
        {
            Transform tmp = list[i];
            int r = Random.Range(i, list.Count);
            list[i] = list[r];
            list[r] = tmp;
        }
    }

	public void LockCursor()
	{
		if (Cursor.lockState != CursorLockMode.Locked)
		{
		Cursor.lockState = CursorLockMode.Locked;
		}
	}

    public void ResetVariables()
    {
        isInstantiating = false;
		instantiateInt = 0;
        instantiateTime = 4.0f;
		totalBotsSpawned += UIController.s.totalBotsSpawnedThisWave;
		print("(PC) totalBotsSpawned: " + totalBotsSpawned);
		print("(PC) totalBotsThisWave: " + UIController.s.totalBotsSpawnedThisWave);
		
		UIController.s.totalBotsSpawnedThisWave = 0;
		Glove.s.coffeeCreated = false;
		powerupsSpawned = 0;

        if (currentSceneName == "L1_MailRoom")
        {
            waveTotal = 3;
            if (waveNumber == 1)
            {
                //L1W1
                botTotal = 12;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.05f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 0;
                coffeeTotal = 0;
                healthTotal = 0;
                botSpawnWeights = new float[] { 1, 0, 0, 0, 0, 0 }; // only mailbot
                holeTotal = 0;
            }
            else if (waveNumber == 2)
            {
                //L1W2
                botTotal = 18;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.05f;
                PUpTimeLength = 10; 
                powerupTotal = 0;
                coffeeTotal = 0;
                healthTotal = 0;
                botSpawnWeights = new float[] { 0.85f, 1, 0, 0, 0, 0 };
                holeTotal = 0;
            }
            else if (waveNumber == 3)
            {
                //L1W3
                botTotal = 22;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.05f;
                PUpTimeLength = 10;
                powerupTotal = 0;
                coffeeTotal = 0;
                healthTotal = 1;
                botSpawnWeights = new float[] { 0.7f, 1, 0, 0, 0, 0 };
                holeTotal = 0;
            }
        }
        else if (currentSceneName == "L2_Reception")
        {
            waveTotal = 5;
            if (waveNumber == 1)
            {
                //L2W1
                botTotal = 12;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.05f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 0;
                coffeeTotal = 0;
                healthTotal = 0;
                // 6 bots - mailbot, clampbot, coffeebot, fembot, destructobot, thermobot
                botSpawnWeights = new float[] { 0, 1, 0, 0, 0, 0 };
                holeTotal = 0;
            }
            else if (waveNumber == 2)
            {
                //L2W2
                botTotal = 15;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.05f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 1;
                coffeeTotal = 1;
                healthTotal = 0;
                botSpawnWeights = new float[] { 0, 0.9f, 1, 0 , 0, 0 };
                holeTotal = 1;
            }
            else if (waveNumber == 3)
            {
                //L2W3
                botTotal = 18;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.05f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 1;
                coffeeTotal = 1;
                healthTotal = 1;
                botSpawnWeights = new float[] { 0, 0.8f, 1, 0, 0, 0 }; // only mailbot
                holeTotal = 4;
            }
            else if (waveNumber == 4)
            {
                //L2W4
                botTotal = 22;
                botsSpawnedThisWave = 0;
                coefficent = 4.5f;
                decayFactor = 0.06f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 1;
                coffeeTotal = 1;
                healthTotal = 1;
                botSpawnWeights = new float[] { 0, 0.7f, 1, 0, 0, 0 }; // only mailbot
                holeTotal = 8;
            }
            else if (waveNumber == 5)
            {
                //L2W5
                botTotal = 30;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.06f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 1;
                coffeeTotal = 0;
                healthTotal = 2;
                botSpawnWeights = new float[] { 0, 0.6f, 0.8f, 1, 0, 0 };
                holeTotal = 15;
            }
        }
        else if (currentSceneName == "L3_Offices")
        {
            waveTotal = 5;
            if (waveNumber == 1)
            {
                //L3W1
                botTotal = 12;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.06f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 1;
                coffeeTotal = 1;
                healthTotal = 2;
                botSpawnWeights = new float[] { 0, 0.8f, 0.9f, 1, 0, 0 };
                holeTotal = 0;
            }
            else if (waveNumber == 2)
            {
                //L3W2
                botTotal = 16;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.06f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 2;
                coffeeTotal = 0;
                healthTotal = 1;
                botSpawnWeights = new float[] { 0, 0.6f, 0, 0.8f, 1, 0 };
                holeTotal = 4;
            }
            else if (waveNumber == 3)
            {
                //L3W3
                botTotal = 120;
                botsSpawnedThisWave = 0;
                coefficent = 5;
                decayFactor = 0.06f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 0;
                coffeeTotal = 0;
                healthTotal = 1;
                botSpawnWeights = new float[] { 0, 0.5f, 0.6f, 0.8f, 1, 0 }; // only mailbot
                holeTotal = 6;
            }
            else if (waveNumber == 4)
            {
                //L3W4
                botTotal = 24;
                botsSpawnedThisWave = 0;
                coefficent = 4.5f;
                decayFactor = 0.05f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 0;
                coffeeTotal = 0;
                healthTotal = 1;
                botSpawnWeights = new float[] { 0, 0.4f, 0, 0.7f, 1, 0 }; // only mailbot
                holeTotal = 12;
            }
            else if (waveNumber == 5)
            {
                //L3W5
                botTotal = 28;
                botsSpawnedThisWave = 0;
                coefficent = 4.5f;
                decayFactor = 0.05f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 3;
                coffeeTotal = 1;
                healthTotal = 1;
                botSpawnWeights = new float[] { 0, 0.6f, 0.8f, 0, 0.9f, 1 }; // only mailbot
                holeTotal = 12;
            }
			else if (waveNumber == 6)
            {
                //L3W6
                botTotal = 30;
                botsSpawnedThisWave = 0;
                coefficent = 4.5f;
                decayFactor = 0.06f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 2;
                coffeeTotal = 0;
                healthTotal = 1;
                botSpawnWeights = new float[] { 0, 0.3f, 0, 0.7f, 0.9f, 1 }; // only mailbot
                holeTotal = 14;
            }
			else if (waveNumber == 7)
            {
                //L3W7
                botTotal = 32;
                botsSpawnedThisWave = 0;
                coefficent = 4.5f;
                decayFactor = 0.06f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 1;
                coffeeTotal = 1;
                healthTotal = 2;
                botSpawnWeights = new float[] { 0, 0.3f, 0.6f, 0.8f, 0, 1 }; // only mailbot
                holeTotal = 16;
            }
			else if (waveNumber == 8)
            {
                //L3W8
                botTotal = 35;
                botsSpawnedThisWave = 0;
                coefficent = 4;
                decayFactor = 0.05f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 2;
                coffeeTotal = 0;
                healthTotal = 2;
                botSpawnWeights = new float[] { 0, 0.5f, 0, 0, 0, 1 }; // only mailbot
                holeTotal = 18;
            }
			else if (waveNumber == 9)
            {
                //L3W9
                botTotal = 50;
                botsSpawnedThisWave = 0;
                coefficent = 4;
                decayFactor = 0.03f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 3;
                coffeeTotal = 2;
                healthTotal = 2;
                botSpawnWeights = new float[] { 0, 0.15f, 0.35f, 0.5f, 0.8f, 1 }; // only mailbot
                holeTotal = 20;
            }
			else if (waveNumber == 10)
            {
			     //L3W10
                botTotal = 80;
                botsSpawnedThisWave = 0;
                coefficent = 4;
                decayFactor = 0.01f;
                PUpTimeLength = 10;  // set to 10 so far for all levels - might change
                powerupTotal = 1;
                coffeeTotal = 1;
                healthTotal = 1;
                botSpawnWeights = new float[] { 0, 0.1f, 0.3f, 0.5f, 0.7f, 1 }; // only mailbot
                holeTotal = 40;
            }
        }
    }
    void EnableStartButtonColliders()
    {
        foreach (Collider col in colChildren)
        {
            col.GetComponent<Collider>().enabled = true;

        }
        foreach (Rigidbody rig in rigidChildren)
        {
            rig.isKinematic = false;
        }
    }

    void DisableStartButtonColliders()
    {
        startCol.GetComponent<Collider>().enabled = false;  // disable collider on parent

        foreach (Collider col in colChildren)
        {
            col.GetComponent<Collider>().enabled = false;  // after explosion, disable colliders so they don't hit the player

        }
        foreach (Rigidbody rig in rigidChildren)
        {
            rig.isKinematic = false;
        }
    }

    public IEnumerator LerpGameObject(GameObject moveThis, Vector3 moveHere, float moveTime)
    {
        t = 0f;
        Vector3 moveFrom = moveThis.transform.position;

        while (t < 1)
        {
            t += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(moveFrom, moveHere, t);
            yield return null;
        }
        //yield return new WaitForSeconds(moveTime);
    }

    public IEnumerator MoveToPosition()
    {
        t = 0f;
        timeToMove = 2.5f;
        UIController.s.ElevatorOpen();
        yield return new WaitForSeconds(5);     // wait for the elevator door to open
        StartCoroutine(LerpGameObject(gameObject, movePlayerHere.transform.position, timeToMove));
        //while (t < 1)
        //{
        //    t += Time.deltaTime / timeToMove;
        //    transform.position = Vector3.Lerp(position_i, movePlayerHere.transform.position, t);
        //    yield return null;
        //}
        yield return new WaitForSeconds(0.5f);
        glove.SetActive(true);
        glove.transform.localEulerAngles = new Vector3(90, 0, 0);   // make sure the glove is not pointed up
        StartCoroutine(UIController.s.SlerpRot(glove, Quaternion.identity, 2.0f));
        UIController.s.ElevatorClose();
        yield return new WaitForSeconds(4);
        if (resetSceneButton) resetSceneButton.SetActive(true);

    }

    public IEnumerator MoveToElevator()
    {
        UIController.s.ElevatorOpen();
        yield return new WaitForSeconds(2);
        Vector3 gloveDownRot = new Vector3(90, 0, 0);
        StartCoroutine(UIController.s.SlerpRot(glove, gloveDownRot, 2.0f));
        glove.SetActive(false);
        t = 0f;
        timeToMove = 3.5f;
        StartCoroutine(LerpGameObject(gameObject, position_i, timeToMove));
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(movePlayerHere.transform.position, position_i, t);    // move back to the elevator
            yield return null;
        }
        //yield return new WaitForSeconds(2);

        UIController.s.ElevatorClose();
        yield return new WaitForSeconds(6);     // wait for the elevator door to close
        gameObject.GetComponent<AudioSource>().enabled = false;  // turn off level music
        yield return new WaitForSeconds(1);
        UIController.s.PlayElevatorMusic();
        yield return new WaitForSeconds(1.5f);
        thisLevelScore = score - GameManager.s.storedScore;
        UIController.s.StartEndMessages();
    }

    bool punchStartOnce = false;
    IEnumerator StartButtonDelay()
    {
        if (punchStartOnce) yield break;
        punchStartOnce = true;
        instantiateTime = 4.0f;
        EnableStartButtonColliders();
        audiosource.PlayOneShot(explosionAudio);
        yield return new WaitForSeconds(0.1f);
        DisableStartButtonColliders();
        yield return new WaitForSeconds(2f);
        punchStartOnce = false;
        Destroy(startButton);

    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        playerScore.text = "Score: " + score;
    }

    //IEnumerator StartMessage()
    //{
    //    yield return new WaitForSeconds(3f);
    //    instructions.SetActive(true);
    //    yield return new WaitForSeconds(5f);
    //    instructions.SetActive(false);
    //}

    bool isInstantiating = false;

    IEnumerator InstantiateEnemyDelay()
    {
        if (isInstantiating)
        {
            print("Cannot instantiate another bot while already instantiating!");
            yield break;
        }

        isInstantiating = true;
        instantiateTime = coefficent * Mathf.Exp(-decayFactor * instantiateInt);
        yield return new WaitForSeconds(instantiateTime);

        HoleValue();
        RandomEnemy();
        if (instantiateInt <= botTotal - 1)
        {
            if (holeBot)
            {
				try
				{
				holeLocationsIndex = Random.Range(0, holeLocations.Count);
                StartCoroutine(InstantiateEnemy(botPrefabs[botTypeIndex], holeLocations[holeLocationsIndex]));
				}
				catch
				{
					print("OOPS!");
					print("bot prefabs length" + botPrefabs.Length);
					print(".. but actual bot prefab index" + botTypeIndex);
					print("holeLocations length" + holeLocations.Count);
					print(".. but actual hole locations index" + holeLocationsIndex);
				}
            }
            else
            {
				try
				{
                StartCoroutine(InstantiateEnemy(botPrefabs[botTypeIndex], spawnLocations[botSpawnIndex].transform));
				}
				catch
				{
					print("OOPS!");
					print("bot prefabs length" + botPrefabs.Length);
					print("actual bot prefab index" + botTypeIndex);
					print("spawnLocations length" + spawnLocations.Count);
					print("actual bot spawn index" + botSpawnIndex);
				}
			}
        }
        else instantiateInt = botTotal - 1;
    }

    void HoleValue()
    {
        float holeRatio = holeTotal / botTotal;
        float rand = Random.value;
        //print("holeTotal = " + holeTotal);
        //print("holeRatio and rand = " + holeRatio + " and " + rand);
        if (rand < holeRatio)
        {
            holeBot = true; // spawn a bot in a hole
        }
        else holeBot = false; // spawn a bot behind something
    }

    void RandomEnemy()
    {
        float rand = Random.value;
        //print("spawn rand = " + rand);
        if (rand <= botSpawnWeights[0])    
        {
            botTypeIndex = 0; // assign MAIL
        }
        else if (rand > botSpawnWeights[0] && rand <= botSpawnWeights[1])
        {
            botTypeIndex = 1; // assign BUSINESS
        }
        else if (rand > botSpawnWeights[1] && rand <= botSpawnWeights[2])
        {
            botTypeIndex = 2; // assign COFFEE
        }
        else if (rand > botSpawnWeights[2] && rand <= botSpawnWeights[3])
        {
            botTypeIndex = 3; // assign FEM
        }
        else if (rand > botSpawnWeights[3] && rand <= botSpawnWeights[4])
        {
            botTypeIndex = 4; // assign DESTRUCTO
        }
        else if (rand > botSpawnWeights[4] && rand <= botSpawnWeights[5])
        {
            botTypeIndex = 5; // assign THERMO 
        }
    }

    public IEnumerator InstantiateEnemy(GameObject bot, Transform spawnLocation)
    {
        
        if (spawnBots)
        {
            GameObject spawnedBotObj = Instantiate(bot, spawnLocation.position, transform.rotation * Quaternion.Euler(0f, 180f, 0f));
			UIController.s.totalBotsSpawnedThisWave++;

            if (holeBot == true)
            {
                //db.cube(spawnLocation.position);
                GameObject correctHole = null;
                if (botTypeIndex == 1)      // clamp bot
                {
                    correctHole = holePrefabs[0];
                    holeForceMag = _holeForceMag;
                    holePos = new Vector3(spawnLocation.position.x, 3.405f, spawnLocation.position.z); // y position is hard coded so it's always on the floor
                }
                else
                {
                    correctHole = holePrefabs[1];
                    holeForceMag = _holeForceMag;
                    holePos = new Vector3(spawnLocation.position.x, 3.405f, spawnLocation.position.z); // y position is hard coded so it's always on the floor
                }
                if (botTypeIndex == 3) // fem bot
                {
                    float xCorr = 0.31707f;
                    holePos = new Vector3(spawnLocation.position.x + xCorr, 3.405f, spawnLocation.position.z); // x correction so the hole is in the middle of the bot
                }
                spawnedBotObj.GetComponent<EnemyController>().canvas.SetActive(false);
                GameObject spawnedHole = Instantiate(correctHole, holePos, Quaternion.identity);
                Collider botCol = spawnedBotObj.GetComponent<Collider>();
                Rigidbody rb = spawnedBotObj.GetComponent<Rigidbody>();
                //rb.constraints=RigidbodyConstraints.FreezeRotation;
                //NavMeshAgent botNavMesh = spawnedBotObj.GetComponent<NavMeshAgent>();
                NavBot navBot = spawnedBotObj.GetComponent<NavBot>();
                botCol.enabled = false;
                navBot.enabled = false;
                rb.useGravity = true;
                //botNavMesh.enabled = false;
                rb.AddForce(Vector3.up * holeForceMag, ForceMode.Impulse);
                //TimeManager.s.SetGameSpeed(0);
                yield return new WaitForSeconds(1.5f);
                botCol.enabled = true;
                yield return new WaitForSeconds(1.0f);
                rb.useGravity = false;
                navBot.enabled = true;

                spawnedBotObj.GetComponent<EnemyController>().canvas.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                Destroy(spawnedHole);

                yield return new WaitForSeconds(1.25f);
                float t = 0;
                float upTime = 0.25f;
                if (spawnedBotObj == null)
                {
                    print("oops, bot is null! ABORT");
                    yield break;
                }
                
                Quaternion currRot = spawnedBotObj.transform.rotation;
                
                Quaternion targetRot = Quaternion.Euler(0, 180, 0);
                while (t < upTime)
                {
                    t += Time.fixedDeltaTime;
					if (spawnedBotObj)
					{
                    spawnedBotObj.transform.rotation = Quaternion.Slerp(currRot, targetRot, t / upTime);
					}
					else print("null!");
                    yield return new WaitForFixedUpdate();
                }
                //botNavMesh.enabled = true;
                //botNavMesh.SetDestination(gameObject.transform.position);
                //if(spawnedBotObj.GetComponent<EnemyController>().isAlive) botNavMesh.enabled = true;
            }

            botsSpawnedThisWave++;
            
			//botSpawnIndex++;
            //if (botSpawnIndex >= spawnLocations.Count)
            //{
            //    botSpawnIndex = 0;
            //}
			botSpawnIndex += Random.Range(1,2);
			botSpawnIndex %= (spawnLocations.Count);
			//print("botSpawnIndex="+botSpawnIndex);
        }

        if (instantiateInt < botTotal-1)
        {
            instantiateInt++;
        }
        isInstantiating = false;
    }

    void InstantiatePowerUp()
    {
        int randomPUpIndex = Random.Range(0, PUpPrefabs.Length);
        Vector3 position = PUpLocations[puIndex].transform.position;
        powerup = Instantiate(PUpPrefabs[randomPUpIndex], position, Quaternion.identity) as GameObject;
		powerupsSpawned++;
        puIndex++;
    }

    void InstantiateHealthPack()
    {
        int randomIndex = Random.Range(0, healthLocations.Count);
        Vector3 position = healthLocations[randomIndex].transform.position;
        health = Instantiate(healthPrefab, position, Quaternion.identity) as GameObject;
    }

    //bool isFlashing = false;

    public IEnumerator PlayerHitFlashDelay(float delayTime)
    {
        playerHitFlash.SetActive(true);
        yield return new WaitForSeconds(delayTime);
        playerHitFlash.SetActive(false);
    }

    bool isPunching = false;
	public int powerupsCollected;
	public int holeLocationsIndex;

	IEnumerator PunchOnce()
    {
        if (isPunching) yield break;
        isPunching = true;
        glovePunchAnim.speed = gloveAnimSpeed;
        glovePunchAnim.SetTrigger("Punch");                 // trigger punching animation
        
        //print("PunchOnce - gloveAnimSpeed = " + gloveAnimSpeed);
        yield return new WaitForSeconds(0.5f);
        isPunching = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" || other.tag == "Explode")
        {
			if(other.tag == "Explode") 
				{
				StartCoroutine(ExplosionHit()); 
				print("Explode!!");
				}


            if (other.GetComponent<EnemyController>().isAlive)
            {
                AddHealth(-10f);
                StartCoroutine(PlayerHitFlashDelay(0.3f));
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

   //     if (other.tag == "Explode")
   //     {
   //         StartCoroutine(ExplosionHit());                 // DestructoBot hit player, activate explosion
			//if (other.GetComponent<EnemyController>().isAlive)
   //         {
   //             AddHealth(-10f);
   //             StartCoroutine(PlayerHitFlashDelay(0.3f));
   //         }

   //         if (currentHealth <= 0)
   //         {
   //             Die();
   //         }
   //     }
    }

    IEnumerator ExplosionHit()
    {
        explode.SetActive(true);
		print("explode active?: " + explode.activeInHierarchy);
        yield return new WaitForSeconds(2f);
        explode.SetActive(false);
    }

    void Die()
    {
        //playerHitFlash.SetActive(true);
        spawnBots = false;                            //stop making bots!
        playerDead = true;                              //we are dead...
        currentHealth = 0;
        //playerHitAnim.SetBool(deathTriggerName, true);
        instantiateTime = 10000f;
    }

    void WaveComplete()
    {
        if (waveNumber < waveTotal)
        {
            waveNumber++;
            UIController.s.NextWaveUISequence(); // display next Wave UI message
        }
    }

    IEnumerator HealingPlayer()
    {
        GameObject poof = Instantiate(healthIncreasePoof, transform.position, Quaternion.identity);
        poof.SetActive(true);
        yield return new WaitForSeconds(5f);
        Destroy(poof);
    }

    IEnumerator GiveHealthPack()
    {
        InstantiateHealthPack();
        needHealth = false;
        yield return new WaitForSeconds(15f);
        needHealth = true;
    }

    IEnumerator SpawnPowerUps()
    {
        waitForSpawnTime = true;
        yield return new WaitForSeconds(spawnTime);
        InstantiatePowerUp();
        waitForSpawnTime = false;
        powerUpInGame = true;
        spawnTime *= 0.5f;
    }

    public void AddHealth(float add)
    {
        UpdateHealth(currentHealth + add);
    }

    public void UpdateHealth(float newHealth)
    {
        if (currentHealth < 0)
        {
            currentHealth = 0; //print("Already Dead");
            return;
        }

        currentHealth = newHealth;//the only place on earth where health can actually get changed

        healthBar.value = currentHealth;

        if (currentHealth <= 0) Die();

    }

    public void ResetScene()
    {
        SceneManager.LoadScene(currentSceneName);
    }

    // Update is called once per frame
    void Update()
    {

        //if (Glove.coffeePowerUp) // && coffeePunch
        //{
        //    glovePunchAnim.SetTrigger("Punch");                 // trigger punching animation
        //    glovePunchAnim.speed = 2f;                          // punch fast!
        //}

        if (Glove.healthPackCollected)
        {
            StartCoroutine(HealingPlayer());
            AddHealth(50f);
            //currentHealth += 50f;                           // increase health!
            //healthBar.value = currentHealth;
            if (currentHealth > maxHealth)                  // don't go over max health
            {
                currentHealth = maxHealth;
            }
            Glove.healthPackCollected = false;
        }
        else healthIncreasePoof.SetActive(false);

        if (Glove.startPunched)
        {
            gameObject.GetComponent<AudioSource>().enabled = true;  // turn on music when start button is hit
            spawnBots = true;
            StartCoroutine(StartButtonDelay());
            Glove.startPunched = false;
        }

        if (playerDead)
        {
            GameObject[] bots = GameObject.FindGameObjectsWithTag("Enemy");  // Player is dead, find all active enemies
            GameObject[] destructoBots = GameObject.FindGameObjectsWithTag("Explode");
            foreach (GameObject bot in bots)
                Destroy(bot);                                // Destroy all of them
            foreach (GameObject dest in destructoBots)         // DestructoBots have a different tag so destroy them too!
                Destroy(dest);
        }

        if (currentHealth < 0.3 * maxHealth && needHealth && Glove.s.healthPacksCollected < healthTOTAL)      // health is low, spawn a healthpack
        {
            StartCoroutine(GiveHealthPack());
        }

        if (!waitForSpawnTime && powerupsSpawned < powerupTotal && !Glove.powerUpActive && spawnBots && !powerUpInGame)
        {
            StartCoroutine(SpawnPowerUps());
        }

        //if (!spawnPowerUp && powerup != null)
        //{
        //    StartCoroutine(PowerUpSpawnTimer());
        //}

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        
        if (Input.GetMouseButtonDown(0))
        {
			if (UIController.s.currentSceneName == "L0_Tutorial")
			{
			LockCursor();
			mouseTimeDown = 1f;
			}
			else if (PauseMenuu.s && !PauseMenuu.s.pauseMenuCanvas.activeInHierarchy)
			{
			LockCursor();
			mouseTimeDown = 1f;
			}
        }

        if (Input.GetMouseButton(0))
        {
            mouseTimeDown += Time.deltaTime;
            if (mouseTimeDown >= 3f)
            {
                mouseTimeDown = 3f;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //print("mouseTimeDown = " + mouseTimeDown);
            gloveAnimSpeed = mouseTimeDown;
            //glovePunchAnim.speed = mouseTimeDown;
            StartCoroutine(PunchOnce());
        }
#elif UNITY_ANDROID
            accelerationZ = -Input.acceleration.z;
            accelerationY = Input.acceleration.y;

            if (accelerationY < -0.7f && accelerationZ > 0.7f)                          // check if the phone is not laying flat on the table
            {
                print("if statement aZ = " + Input.acceleration.z);
                //StartCoroutine(GetAcceleration());
                gloveAnimSpeed = (5f * accelerationZ - 1f) / 3f;    // accelerations are between 1 and 2, so this makes the range of speeds between 1ish and 3
                print("accelerationZ = " + accelerationZ);
                if (gloveAnimSpeed < 0)
                {
                    gloveAnimSpeed = -1 * gloveAnimSpeed;
                }
                StartCoroutine(PunchOnce());
            }
#endif

        if (instantiateTime > 0 && !isInstantiating && spawnBots)
        {
            StartCoroutine(InstantiateEnemyDelay());
        }

        if (botsSpawnedThisWave >= botTotal)  // stop spawning bots if we have spawned enough
        {
            spawnBots = false;
            if (!playerDead && GameObject.FindWithTag("Enemy") == null)            // if we are still alive and there are no more bots, we won!
            {
                waveCompleted = true;
                botsSpawnedThisWave = 0; // this is just to stop it from looping again
                if (waveNumber == waveTotal)
                {
                    levelCompleted = true;
                    noMoreWaves = true;
                }
            }
        }

        if (levelCompleted && noMoreWaves && once1)
        {
			PauseMenuu.s.pauseButton.SetActive(false);
            StartCoroutine(MoveToElevator());
            levelCompleted = false;
            once1 = false;
        }

        if (waveCompleted && !noMoreWaves && !spawnBots)
        {
            WaveComplete();
            //StartCoroutine(MoveToElevator());
            waveCompleted = false;
        }
        //print("time = " + Time.time);
    }

}